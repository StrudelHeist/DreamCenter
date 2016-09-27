using CommonLib;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SharedLib
{
    public class MessageUtility
    {
        public delegate void MessageHandler(object message);
        public delegate void MessageHandlerTCP(object message, TCP_Message tcpMsg);

        private static byte[] SerializeData(object dataToSerialize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(dataToSerialize.GetType());
                serializer.Serialize(ms, dataToSerialize);
                return ms.ToArray();
            }
        }
        private static object DeserializeData(byte[] dataToDeserialize, Type type)
        {
            using (MemoryStream ms = new MemoryStream(dataToDeserialize))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(ms);
            }
        }

        public static byte[] BuildMessage(object message)
        {
            // Get information about class name
            string className = message.GetType().ToString();
            byte[] nameAsBytes = Encoding.ASCII.GetBytes(className);
            byte[] nameSize = BitConverter.GetBytes(nameAsBytes.Length); // THIS SHOULD BE 4 BYTES

            // Get message bytes
            byte[] messageAsBytes = SerializeData(message);

            // Combine data into array
            byte[] data = new byte[nameSize.Length + nameAsBytes.Length + messageAsBytes.Length];
            Buffer.BlockCopy(nameSize, 0, data, 0, nameSize.Length);
            Buffer.BlockCopy(nameAsBytes, 0, data, nameSize.Length, nameAsBytes.Length);
            Buffer.BlockCopy(messageAsBytes, 0, data, nameSize.Length + nameAsBytes.Length, messageAsBytes.Length);

            return data;
        }
        public static object UnpackMessage(byte[] rawMessage, out string className)
        {
            // Get the class name
            byte[] nameSizeBytes = new byte[4];
            Buffer.BlockCopy(rawMessage, 0, nameSizeBytes, 0, 4);
            int nameSize = BitConverter.ToInt32(nameSizeBytes, 0);
            byte[] nameBytes = new byte[nameSize];
            Buffer.BlockCopy(rawMessage, 4, nameBytes, 0, nameSize);
            className = Encoding.ASCII.GetString(nameBytes);

            // Get the message
            Type elementType = Type.GetType(className);
            byte[] dataBytes = new byte[rawMessage.Length - 4 - nameSize];
            Buffer.BlockCopy(rawMessage, 4 + nameSize, dataBytes, 0, rawMessage.Length - nameSize - 4);
            return DeserializeData(dataBytes, elementType);
        }
    }
}
