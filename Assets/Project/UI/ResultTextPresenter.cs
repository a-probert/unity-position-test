using JetBrains.Annotations;
using TMPro;
using UniRx;

namespace Project.UI
{
    [UsedImplicitly]
    public sealed class ResultTextPresenter
    {
        private TMP_Text _text;
        private RPSModel _rpsModel;

        public ResultTextPresenter(TMP_Text text, RPSModel rpsModel)
        {
            _text = text;
            _rpsModel = rpsModel;
            _rpsModel.resultsText.Subscribe(ShowResultsText);
        }

        private void ShowResultsText(string s)
        {
            _text.text = s;
        }
    }
}