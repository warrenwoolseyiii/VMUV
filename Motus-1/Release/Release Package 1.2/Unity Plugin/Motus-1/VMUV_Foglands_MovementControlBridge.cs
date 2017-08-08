using System.Collections;
using System.Collections.Generic;
using Motus_1_Plugin;
using UnityEngine;
namespace MichaelWolf
{
	public class VMUV_Foglands_MovementControlBridge : MonoBehaviour
	{
	    [SerializeField] private FogLands_PlayerMovementControl _fogLandsPlayerMovement;
	    public float offsetRotation = 90f;
	    public bool enableHandSteering = false;
	    public bool enableHeadSteering = false;
        public bool enableViveTrackerSteering = true;
        public bool enableAutoOrient = true;
	    public float speed = 2f;
        void Start()
        {
            // Call this function once to initialize the various modules of the 
            // Motus-1 plugin.
            PluginInterface.Initialize();

            // Set this value to enable steering
            PluginInterface.enableHandSteering = enableHandSteering;
            PluginInterface.enableHeadSteering = enableHeadSteering;
            PluginInterface.enableViveTrackerSteering = enableViveTrackerSteering;
            PluginInterface.autoOrientEnable = enableAutoOrient;

            if (!_fogLandsPlayerMovement) _fogLandsPlayerMovement = GetComponent<FogLands_PlayerMovementControl>();
        }
        private void FixedUpdate()
	    {
            // Call this function in update to continue reading data from the 
            // Motus-1 hardware device
            PluginInterface.Service();
	        PluginInterface.enableHeadSteering = enableHeadSteering;
            PluginInterface.enableHandSteering = enableHandSteering;
            PluginInterface.enableViveTrackerSteering = enableViveTrackerSteering;

            if (enableHeadSteering)
                offsetRotation = 90f;
            else if (enableViveTrackerSteering)
                offsetRotation = 0f;
            else
                offsetRotation = 90f;

            Vector3 xz = PluginInterface.GetXZVector();
            Quaternion rot = PluginInterface.GetCharacterRotation();
            Quaternion fixedOffset = Quaternion.Euler(0f, offsetRotation, 0f);
            Vector3 vect = rot * xz;
            vect = fixedOffset * vect;

            _fogLandsPlayerMovement.CharacterControllerFixedUpdate(vect, speed);
        }
	}
}