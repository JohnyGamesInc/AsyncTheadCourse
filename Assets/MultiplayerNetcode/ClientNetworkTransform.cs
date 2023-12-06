using Unity.Netcode.Components;


namespace MultiplayerNetcode
{
    
    public class ClientNetworkTransform : NetworkTransform
    {
        
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
        
        
    }
}