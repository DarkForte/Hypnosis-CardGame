using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class RandomAIPlayer : HumanPlayer
{
    public override IEnumerator SelectCard(GameController gameController)
    {
        for(int i=1; i<=5; i++)
        {
            CardType nowCard = CardPool[p_CardPool];
            p_CardPool++;

            if (i <= 3)
                NowCards.Enqueue(nowCard);
        }

        gameController.remoteCardReady = true;

        yield return null;
    }
}

