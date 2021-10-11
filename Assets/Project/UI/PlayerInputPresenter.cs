using JetBrains.Annotations;
using TMPro;
using UniRx;

namespace Project.UI
{
    [UsedImplicitly]
    public sealed class PlayerInputPresenter
    {
        private RPSModel _rpsModel;
        private TMP_InputField _field;
        public PlayerInputPresenter(TMP_InputField field, RPSModel RPSModel)
        {
            _rpsModel = RPSModel;
            _rpsModel._playerSelection.Subscribe(OnPlayerSelectionChange);
            _field = field;
            _field.onSubmit.AddListener(OnPlayerInputSubmit);
        }

        private void OnPlayerSelectionChange(RPSModel.GameSymbol symbol)
        {
            if (symbol == RPSModel.GameSymbol.None && _field != null)
                _field.text = string.Empty;
        }

        private void OnPlayerInputSubmit(string input)
        {
            input = input.ToLower();
            _rpsModel.ParsePlayerInput(input);
        }
    }
}