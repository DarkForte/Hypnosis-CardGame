using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class RemotePlayer : Player
{
    public override void Play(GameController gameController)
    {
        gameController.GameState = new GameStateRemoteTurn(gameController);
    }

    public override IEnumerator SelectCard(GameController gameController, UIController uiController)
    {
        yield return null;
    }
}

