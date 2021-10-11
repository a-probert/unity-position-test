using JetBrains.Annotations;
using TMPro;
using UniRx;
using Cysharp.Threading.Tasks;
using System;

namespace Project.UI
{
    [UsedImplicitly]
    public sealed class CountdownPresenter
    {
        private TMP_Text _text;
        private RPSModel _rpsModel;
        public CountdownPresenter(TMP_Text text, RPSModel rpsModel)
        {
            _text = text;
            _rpsModel = rpsModel;
            _rpsModel.countdownString.Subscribe(UpdateCountdownText);
        }

        private void UpdateCountdownText(string s)
        {
            _text.text = s;
        }
    }
}