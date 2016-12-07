using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class RemotePlayer : Player
{
    public override void Play(GameController gameController)
    {
        throw new NotImplementedException();
    }

    public override IEnumerator SelectCard(GameController gameController)
    {
        yield return null;
    }
}

