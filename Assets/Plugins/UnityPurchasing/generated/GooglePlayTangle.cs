#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("lGq8TtnNzg0FxtOPI9k/8Jrdp62LAtAaI3WbMiKUNu5iDUa1Yfv0Sv1HP32Tiq37Qq7q4phSBeigmd6d62HvklQ7uxL7Xx8dtuxzBJxvVh01hwQnNQgDDC+DTYPyCAQEBAAFBocECgU1hwQPB4cEBAWK0pMAF2Yb+551B1lHdP8ylMLvfn47thAvt3IeBE9GAx1nB3pPq7HQFZfSb+F+s3WUo63Qs6C6z1JvVgvpIMzW8XT0cZpklevL2/bka9oeZpaBWSXC45URiJlqNgANcH/hlB980zlv+wE3PQLDO5ar8WkL/3WBAZeo4O+5dp9GxzEf7yzLjHSS70wiZDtv0RTSRQ5YrvkWgKviKt3Ys1k7EXBWcx7x3U4gfNLNetFh5gcGBAUE");
        private static int[] order = new int[] { 6,4,8,9,6,6,9,13,8,9,11,12,13,13,14 };
        private static int key = 5;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
