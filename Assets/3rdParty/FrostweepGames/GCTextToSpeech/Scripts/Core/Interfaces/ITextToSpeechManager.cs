using System;

namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech
{
    public interface ITextToSpeechManager
    {
        event Action<GetVoicesResponse> GetVoicesSuccessEvent;
        event Action<PostSynthesizeResponse> SynthesizeSuccessEvent;

        event Action<string> GetVoicesFailedEvent;
        event Action<string> SynthesizeFailedEvent;

        string PrepareLanguage(Enumerators.LanguageCode lang);

        long GetVoices(GetVoicesRequest getVoicesRequest);
        long Synthesize(PostSynthesizeRequest synthesizeRequest);
        void CancelRequest(long requestId);
        void CancelAllRequests();
    }
}