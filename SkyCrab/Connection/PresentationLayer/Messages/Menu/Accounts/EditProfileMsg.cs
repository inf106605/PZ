using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Players;
using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.EDIT_PROFILE"/></para>
    /// <para>Data type: <see cref="PlayerProfile"/> (without login, registration and lastActivity) If a string is empty, the corresponding property will not change.</para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_LOGGED2"/>, <see cref="ErrorCode.NICK_IS_TOO_SHITTY"/>, <see cref="ErrorCode.EMAIL_OCCUPIED2"/></para>
    /// </summary>
    public sealed class EditProfileMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.EDIT_PROFILE; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            PlayerProfile playerProfile = PlayerProfileTranscoder.Get.Read(connection);
            return playerProfile;
        }

        public static MessageInfo? SyncPost(MessageConnection connection, PlayerProfile playerProfile, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, playerProfile, callback, state), timeout);
        }

        public static void AsyncPost(MessageConnection connection, PlayerProfile playerProfile, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (writingBlock) =>
            {
                PlayerProfileTranscoder.Get.Write(connection, writingBlock, playerProfile);
            };
            connection.PostNewMessage(MessageId.EDIT_PROFILE, messageProc, callback, state);
        }

    }
}
