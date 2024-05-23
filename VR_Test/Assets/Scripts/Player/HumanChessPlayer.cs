using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Represents a human player.
/// </summary>
public class HumanChessPlayer : ConcreteChessPlayer
{
    /********************************** FIELDS ************************************/
    private bool isMoving = false;
    private Coroutine moveCoro = null;
    
    public enum MoveState
    {
        MovingToSrc,
        MovingToDst,
        Finished
    }

    
    private MoveState moveState = MoveState.MovingToSrc;
    private Vector2Int src = new Vector2Int(0, 0);
    private Vector2Int dst = new Vector2Int(0, 0);

    /********************************** FROM ConcreteChessPlayer ************************************/

    public override bool hasFinishedMakingMove()
    {
        return !isMoving;
    }

    /// <summary>
    /// Cancel the move
    /// </summary>
    public override void reset()
    {
        isMoving = false;

        // stop the move coroutine.
        if(moveCoro != null)
        {
            this.StopCoroutine(moveCoro);
            moveCoro = null;
        }
    }

    protected override void internalStartMakingMove(in Board board)
    {
        Utility.MyDebugAssert(isMoving == false, "internalStartMakingMove() called when already moving.");

        isMoving = true;
        // reset the moving state.
        moveState = MoveState.MovingToSrc;
        Game.gameSingleton.camMgr.onFocalPointMove(ModelsManager.BoardPositionToWorld(src));

        // the actual work is done in the coroutine per frame.
        moveCoro = this.StartCoroutine(humanMoveCoro(board));
    }

    /********************************** HELPERS ************************************/

    /// <summary>
    /// Whenever this coroutine is resumed (per frame), 
    /// it checks the user input and updates this.move.
    /// When it sees an end move, it completes making the move.
    /// 
    /// </summary>
    /// <param name="board">reference to the current board</param>
    private IEnumerator humanMoveCoro(Board board)
    {
        // Every frame, perfrom checkUserInputPerFrame()
        // If it does not return true, i.e., the end move is not performed,
        // then it suspends and wait for execution in the next frame.
        while (!checkUserInputPerFrame(board))
        {
            yield return null;
        }
        
        // Otherwise, it completes making the move by setting isMoving = false.
        isMoving = false;
    }

    /// <summary>
    /// Interpret the user's physical action to dealtPos
    /// </summary>
    /// <param name="currentAction"></param>
    /// <returns></returns>
    private Vector2Int interpretAction(InputManager.GestureMeaning currentAction)
    {
        Vector2Int deltaPos = Vector2Int.zero;

        switch (currentAction)
        {
            case InputManager.GestureMeaning.FORWARD:
                deltaPos.y = 1;
                break;
            case InputManager.GestureMeaning.BACK:
                deltaPos.y = -1;
                break;
            case InputManager.GestureMeaning.LEFT:
                deltaPos.x = -1;
                break;
            case InputManager.GestureMeaning.RIGHT:
                deltaPos.x = 1;
                break;
            case InputManager.GestureMeaning.START:
                moveState = MoveState.MovingToDst;
                break;
            case InputManager.GestureMeaning.END:
                moveState = MoveState.Finished;
                break;
            default:
                break;
        }

        return deltaPos;
    }


    /// <summary>
    /// Is called every frame during the human's making a move.
    /// 
    /// Checks the user input and updates this.move.
    /// When it sees an end move, returns true.
    /// 
    /// When it sees the action timer is fired, then 
    ///     1. it sets this.move to forfeited.
    ///     2. returns true.
    /// </summary>
    /// <param name="board">reference to the current board</param>
    /// <returns>true iff an end move has been performed by the user.</returns>
    private bool checkUserInputPerFrame(Board board)
    {
        var currentUserInput = Game.gameSingleton.inputMgr.getCurrentMove();
        // var currentUserInput = Game.gameSingleton.inputMgr.getCurrentGesture();

        var delta = interpretAction(currentUserInput);
        
        switch (currentUserInput)
        {
            case InputManager.GestureMeaning.START:
                dst = src;
                moveState = MoveState.MovingToDst;
                break;
            case InputManager.GestureMeaning.END:
                moveState = MoveState.Finished;
                break;
        }

        Vector2Int temp = Vector2Int.zero;
        switch (moveState)
        {
            case MoveState.MovingToDst:
                temp = dst + delta;
                if (Board.isPosInBoard(temp))
                {
                    dst = temp;
                }
                // update the camera
                Game.gameSingleton.camMgr.onFocalPointMove
                (
                    ModelsManager.BoardPositionToWorld(dst)
                );
                break;
            case MoveState.MovingToSrc:
                temp = src + delta;
                if (Board.isPosInBoard(temp))
                {
                    src = temp;
                }
                // update the camera
                Game.gameSingleton.camMgr.onFocalPointMove
                (
                    ModelsManager.BoardPositionToWorld(src)
                );
                break;
            case MoveState.Finished:
                this.move = new ChessMove(getSide(), src, dst);

                if(!Game.gameSingleton.board.checkMove(move))
                {
                    moveState = MoveState.MovingToSrc;
                    src = dst;
                    return false;
                }

                return true;
        }

        return false;

    }

}