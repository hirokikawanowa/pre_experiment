using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VRMoL.Core;

namespace VRMoL.Interaction
{
    public class DoorController : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
    {
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float openSpeed = 2f;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;
        [SerializeField] private bool isLocked = false;

        private AudioSource audioSource;
        private bool isOpen = false;
        private float currentAngle = 0f;
        private Vector3 originalRotation;

        protected override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            originalRotation = transform.localEulerAngles;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (!isLocked)
            {
                base.OnSelectEntered(args);
                ToggleDoor();
            }
        }

        private void ToggleDoor()
        {
            isOpen = !isOpen;
            PlayDoorSound(isOpen);
        }

        private void Update()
        {
            if (isOpen)
            {
                currentAngle = Mathf.Lerp(currentAngle, openAngle, Time.deltaTime * openSpeed);
            }
            else
            {
                currentAngle = Mathf.Lerp(currentAngle, 0f, Time.deltaTime * openSpeed);
            }

            transform.localEulerAngles = originalRotation + new Vector3(0f, currentAngle, 0f);
        }

        private void PlayDoorSound(bool opening)
        {
            if (audioSource != null)
            {
                AudioClip clip = opening ? openSound : closeSound;
                if (clip != null)
                {
                    audioSource.PlayOneShot(clip);
                }
            }
        }

        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }

        public bool IsOpen()
        {
            return isOpen;
        }

        public void ForceClose()
        {
            isOpen = false;
            PlayDoorSound(false);
        }
    }
} 