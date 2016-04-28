namespace SkyCrab.Connection.PresentationLayer.MessageConnections
{
    public delegate void AnswerCallback(MessageInfo? answer, object state);

    internal struct AnswerCallbackWithState
    {
        public AnswerCallback answerCallback;
        public object state;
    }
}
