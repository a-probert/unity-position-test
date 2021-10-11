using JetBrains.Annotations;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

namespace Project.UI
{
    public abstract class SymbolPresenter
    {
        protected RPSModel _rpsModel;
        protected Image _image;
        protected SymbolPresenter(Image image, RPSModel rpsModel)
        {
            _rpsModel = rpsModel;
            _image = image;
        }

        protected void UpdateSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }
    }

    [UsedImplicitly]
    public sealed class PlayerSymbolPresenter : SymbolPresenter
    {
        public PlayerSymbolPresenter(Image image, RPSModel rpsModel) : base(image, rpsModel)
        {
            _rpsModel.playerSprite.Subscribe(UpdateSprite);
        }
    }

    [UsedImplicitly]
    public sealed class OpponentSymbolPresenter : SymbolPresenter
    {
        public OpponentSymbolPresenter(Image image, RPSModel rpsModel) : base(image, rpsModel)
        {
            _rpsModel.aiSprite.Subscribe(UpdateSprite);
        }
    }
}