using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using static SkyCrab.Connection.PresentationLayer.MessageConnection;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.REGISTER"/></para>
    /// <para>Data type: <see cref="PlayerProfile"/> (without nick, registration and lastActivity)</para>
    /// <para>Passible answers: <see cref="LoginOkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.LOGIN_OCCUPIED"/>, <see cref="ErrorCode.PASSWORD_TOO_SHORT"/>, <see cref="ErrorCode.EMAIL_OCCUPIED"/></para>
    /// </summary>
    public sealed class RegisterMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.REGISTER; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            PlayerProfile profile = PlayerProfileTranscoder.Get.Read(connection);
            return profile;
        }
        
        public static MessageInfo? SyncPostRegister(MessageConnection connection, PlayerProfile playerProfile, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostRegister(connection, playerProfile, callback, state), timeout);
        }

        public static void AsyncPostRegister(MessageConnection connection, PlayerProfile playerProfile, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (writingBlock) =>
            {
                PlayerProfileTranscoder.Get.Write(connection, writingBlock, playerProfile);
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.REGISTER, messageProc);
        }

    }
}
