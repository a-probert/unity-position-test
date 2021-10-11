using JetBrains.Annotations;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using UniRx;
using System;
using UnityEngine;

[UsedImplicitly]
public sealed class RPSModel
{
    private readonly Sprite _questionmarkSprite;
    private readonly Sprite _rockSprite;
    private readonly Sprite _paperSprite;
    private readonly Sprite _scissorsSprite;

    public ReactiveProperty<GameSymbol> _playerSelection{ get; private set; }
    private GameSymbol _aiSelection;

    public ReactiveProperty<string> countdownString { get; private set; }
    public ReactiveProperty<string> resultsText { get; private set; }
    public ReactiveProperty<Sprite> playerSprite { get; private set; }
    public ReactiveProperty<Sprite> aiSprite { get; private set; }

    public enum GameSymbol
    {
        None = -1,
        Rock = 0,
        Paper = 1,
        Scissors = 2
    }

    public RPSModel(Sprite questionmarkSprite, Sprite rockSprite, Sprite paperSprite, Sprite scissorsSprite)
    {
        _questionmarkSprite = questionmarkSprite;
        _rockSprite = rockSprite;
        _paperSprite = paperSprite;
        _scissorsSprite = scissorsSprite;

        countdownString = new ReactiveProperty<string>();
        resultsText = new ReactiveProperty<string>();
        playerSprite = new ReactiveProperty<Sprite>();
        aiSprite = new ReactiveProperty<Sprite>();
        _playerSelection = new ReactiveProperty<GameSymbol>();

        ResetGameValues();
    }

    private void ResetGameValues()
    {
        countdownString.SetValueAndForceNotify(string.Empty);
        resultsText.SetValueAndForceNotify(string.Empty);
        _playerSelection.SetValueAndForceNotify(GameSymbol.None);
        _aiSelection = GameSymbol.None;
        ShowSymbols();
    }

    private async void StartGame()
    {
        _aiSelection = await GetAISelectionAsync(UnityWebRequest.Get(Consts.Random_Gen_URL));

        await StartCountdown();

        await RevealGameResults();

        await UniTask.Delay(TimeSpan.FromSeconds(Consts.Reset_Game_Delay));
        ResetGameValues();
    }

    public void ParsePlayerInput(string input)
    {
        if(_playerSelection.Value != GameSymbol.None)
        {
            _playerSelection.Value = SetPlayerInput(input);
            if (_playerSelection.Value != GameSymbol.None)
                StartGame();
        }
    }

    public GameSymbol SetPlayerInput(string input)
    {
        int index = -1;
        for (int i = 0; i < Consts.Input_Strings.Length; i++)
        {
            if (input == Consts.Input_Strings[i])
            {
                index = i;
                break;
            }
        }

        return (GameSymbol)index;
    }

    async UniTask<GameSymbol> GetAISelectionAsync(UnityWebRequest req)
    {
        try
        {
            var op = await req.SendWebRequest();
            var result = op.downloadHandler.text;
            //TODO:Temp code below, was having an issue with the built in json parser. Need to replace.
            result = result.Replace("[", "").Replace("]", "");
            return (GameSymbol)uint.Parse(result);
        }
        catch(Exception e)
        {
            Debug.LogError($"Error when getting AI Selection, returning a default value - {e}");
            return GameSymbol.Paper;
        }
    }

    private async UniTask StartCountdown()
    {
        for (int i = 0; i <= Consts.Countdown_Seconds; i++)
        {
            countdownString.SetValueAndForceNotify($"{Consts.Countdown_Seconds - i}");
            if (i != Consts.Countdown_Seconds)
                await UniTask.Delay(TimeSpan.FromSeconds(1));
        }
    }

    async private UniTask RevealGameResults()
    {
        ShowSymbols();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));
        resultsText.SetValueAndForceNotify(UpdateResultsText(_playerSelection.Value, _aiSelection));
    }

    private void ShowSymbols()
    {
        ShowSymbol(_playerSelection.Value, playerSprite);
        ShowSymbol(_aiSelection, aiSprite);
    }

    private void ShowSymbol(GameSymbol symbol, ReactiveProperty<Sprite> spriteProperty)
    {
        switch (symbol)
        {
            case GameSymbol.None:
                spriteProperty.SetValueAndForceNotify(_questionmarkSprite);
                break;
            case GameSymbol.Rock:
                spriteProperty.SetValueAndForceNotify(_rockSprite);
                break;
            case GameSymbol.Paper:
                spriteProperty.SetValueAndForceNotify(_paperSprite);
                break;
            case GameSymbol.Scissors:
                spriteProperty.SetValueAndForceNotify(_scissorsSprite);
                break;
        }
    }

    public string UpdateResultsText(GameSymbol playerSymbol, GameSymbol aiSymbol)
    {
        //TODO: Make this mess better.
        switch (playerSymbol)
        {
            case GameSymbol.Rock:
                switch (aiSymbol)
                {
                    case GameSymbol.Rock:
                        return Consts.Tie_String;
                    case GameSymbol.Paper:
                        return Consts.AI_Win_String;
                    case GameSymbol.Scissors:
                        return Consts.Player_Win_String;
                }
                break;
            case GameSymbol.Paper:
                switch (aiSymbol)
                {
                    case GameSymbol.Rock:
                        return Consts.Player_Win_String;
                    case GameSymbol.Paper:
                        return Consts.Tie_String;
                    case GameSymbol.Scissors:
                        return Consts.AI_Win_String;
                }
                break;
            case GameSymbol.Scissors:
                switch (aiSymbol)
                {
                    case GameSymbol.Rock:
                        return Consts.AI_Win_String;
                    case GameSymbol.Paper:
                        return Consts.Player_Win_String;
                    case GameSymbol.Scissors:
                        return Consts.Tie_String;
                }
                break;
        }

        return string.Empty;
    }
}
