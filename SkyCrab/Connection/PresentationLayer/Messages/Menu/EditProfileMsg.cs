using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;
using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.EDIT_PROFILE"/></para>
    /// <para>Data type: <see cref="PlayerProfile"/> (without login, registration and lastActivity) If a string is empty, the corresponding property will not change.</para>
    /// <para>Passible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NICK_IS_TOO_SHITTY"/>, <see cref="ErrorCode.PASSWORD_TOO_SHORT2"/>, <see cref="ErrorCode.EMAIL_OCCUPIED2"/></para>
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

        public static MessageInfo? SyncPostEditProfile(MessageConnection connection, PlayerProfile playerProfile, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostEditProfile(connection, playerProfile, callback, state), timeout);
        }

        public static void AsyncPostEditProfile(MessageConnection connection, PlayerProfile playerProfile, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (writingBlock) =>
            {
                PlayerProfileTranscoder.Get.Write(connection, writingBlock, playerProfile);
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.EDIT_PROFILE, messageProc);
        }

    }
}
