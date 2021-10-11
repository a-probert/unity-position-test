using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Project.UI;
using UnityEngine.Networking;
using UnityEngine.TestTools;
using Zenject;

namespace Project.Tests.Play
{
    internal sealed class Example : ZenjectIntegrationTestFixture
    {
        [Inject] private RPSModel rpsModel;
        [Inject] private PlayerInputPresenter playerInputPresenter;
        
        [Inject] private PlayerSymbolPresenter playerSymbolPresenter;
        [Inject] private OpponentSymbolPresenter opponentSymbolPresenter;

        [Inject] private CountdownPresenter countdownPresenter;
        [Inject] private ResultTextPresenter resultPresenter;
        
        [SetUp]
        public void CommonInstall()
        {
            PreInstall();
            UIInstaller.Install(Container);
            PostInstall();
            Container.Inject(this);
        }

        [UnityTest]
        public IEnumerator Installation_Succeeds()
        {
            yield break;
        }

        [UnityTest]
        public IEnumerator Get_Google_Succeeds() => UniTask.ToCoroutine(async () =>
        {
            var request = await UnityWebRequest.Get("www.google.com").SendWebRequest();
            Assert.That(request.result, Is.EqualTo(UnityWebRequest.Result.Success));
        });

        //I was having issues getting the test to run, zenject was throwing errors and warnings.
        [UnityTest]
        public void ValidateInput()
        {
            Assert.That(rpsModel.SetPlayerInput("rock"), Is.EqualTo(RPSModel.GameSymbol.Rock));
            Assert.That(rpsModel.SetPlayerInput("paper"), Is.EqualTo(RPSModel.GameSymbol.Paper));
            Assert.That(rpsModel.SetPlayerInput("scissors"), Is.EqualTo(RPSModel.GameSymbol.Scissors));
            Assert.That(rpsModel.SetPlayerInput("invalid"), Is.EqualTo(RPSModel.GameSymbol.None));
            
        }

        [UnityTest]
        public void ValidateGameResults()
        {
            Assert.That(rpsModel.UpdateResultsText(RPSModel.GameSymbol.Rock, RPSModel.GameSymbol.Rock),
                Is.EqualTo(Consts.Tie_String));
            Assert.That(rpsModel.UpdateResultsText(RPSModel.GameSymbol.Rock, RPSModel.GameSymbol.Paper),
                Is.EqualTo(Consts.AI_Win_String));
            Assert.That(rpsModel.UpdateResultsText(RPSModel.GameSymbol.Rock, RPSModel.GameSymbol.Scissors),
                Is.EqualTo(Consts.Player_Win_String));

            Assert.That(rpsModel.UpdateResultsText(RPSModel.GameSymbol.Paper, RPSModel.GameSymbol.Rock),
                Is.EqualTo(Consts.Player_Win_String));
            Assert.That(rpsModel.UpdateResultsText(RPSModel.GameSymbol.Paper, RPSModel.GameSymbol.Paper),
                Is.EqualTo(Consts.Tie_String));
            Assert.That(rpsModel.UpdateResultsText(RPSModel.GameSymbol.Paper, RPSModel.GameSymbol.Scissors),
                Is.EqualTo(Consts.AI_Win_String));

            Assert.That(rpsModel.UpdateResultsText(RPSModel.GameSymbol.Scissors, RPSModel.GameSymbol.Rock),
                Is.EqualTo(Consts.AI_Win_String));
            Assert.That(rpsModel.UpdateResultsText(RPSModel.GameSymbol.Scissors, RPSModel.GameSymbol.Paper),
                Is.EqualTo(Consts.Player_Win_String));
            Assert.That(rpsModel.UpdateResultsText(RPSModel.GameSymbol.Scissors, RPSModel.GameSymbol.Scissors),
                Is.EqualTo(Consts.Tie_String));
        }

        [UnityTest]
        public IEnumerator ValidateRandomResponse() => UniTask.ToCoroutine(async () =>
        {
            var request = await UnityWebRequest.Get(Consts.Random_Gen_URL).SendWebRequest();
            Assert.That(request.downloadHandler.text, Does.Match(@"^\[[123]\]$"));
        });
    }
}