﻿using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankHealth : MonoBehaviour
    {
        public PhotonView PV;
        
        public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
        public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
        public Image m_FillImage;                           // The image component of the slider.
        public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
        public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
        public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.
        public bool m_Dead;                                // Has the tank been reduced beyond zero health yet?


        private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
        private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
        private float m_CurrentHealth;                      // How much health the tank currently has.


        private void Awake ()
        {
            // 폭발 조립식을 인스턴스화하고 그 위에 있는 입자 시스템에 대한 참조를 가져옵니다.
            // Instantiate the explosion prefab and get a reference to the particle system on it.
            m_ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem>(); 
            // m_ExplosionParticles = PhotonNetwork.Instantiate("CompleteTankExplosion", Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();;
                    

            // 인스턴스화된 프리팹에서 오디오 소스에 대한 참조를 가져옵니다.
            // Get a reference to the audio source on the instantiated prefab.
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

            // 필요할 때 활성화할 수 있도록 프리팹을 비활성화합니다.
            // Disable the prefab so it can be activated when it's required.
            m_ExplosionParticles.gameObject.SetActive(false);
        }


        private void OnEnable()
        {
            // When the tank is enabled, reset the tank's health and whether or not it's dead.
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;

            // Update the health slider's value and color.
            UpdateHealthUI();
        }


        [PunRPC]
        public void TakeDamage(float amount)
        {
            Debug.Log($"TakeDamage 동기화!, Damage:{amount}");
            
            // Reduce current health by the amount of damage done.
            m_CurrentHealth -= amount;

            // 모든 클라이언트에게 체력 업데이트
            PV.RPC("UpdateHealth", RpcTarget.AllBuffered, m_CurrentHealth);

            // If the current health is at or below zero and it has not yet been registered, call OnDeath.
            if (m_CurrentHealth <= 0f && !m_Dead)
            {
                OnDeath();
            }
        }

        [PunRPC]
        private void UpdateHealth(float newHealth)
        {
            m_CurrentHealth = newHealth;
            UpdateHealthUI();
        }
        
        private void UpdateHealthUI()
        {
            // Set the slider's value appropriately.
            m_Slider.value = m_CurrentHealth;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
            m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }

        
        private void OnDeath ()
        {
            // Debug.Log($"OnDeath(사망처리) 동기화!");
            
            // Set the flag so that this function is only called once.
            m_Dead = true;

            // Move the instantiated explosion prefab to the tank's position and turn it on.
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive (true);

            // Play the particle system of the tank exploding.
            m_ExplosionParticles.Play ();

            // Play the tank explosion sound effect.
            m_ExplosionAudio.Play();

            // 데스 1 증가
            if (PV.IsMine)
            {
                PlayerManager.instance.LocalPlayerDeath += 1;
                
                // Turn the tank off.
                PV.RPC("DestroyTank", RpcTarget.All);
            }

            // 사망하면 방에서 추방되도록 구현
             // if (PV.IsMine)
             // {
             //     // PlayerManager.instance.LocalPlayerDeath++; // 데스 증가
             //     PhotonNetwork.LeaveRoom();
             //     ScenesManager.instance.LoadScene("1.TitleScene");
             // }
        }

        [PunRPC]
        private void DestroyTank()
        {
            gameObject.SetActive (false);
            InGameAlivePlayers.instance.AlivePlayerCount -= 1;
        }

        // private IEnumerator DeadRoutine()
        // {
        //     yield return new WaitForSeconds(3f);
        // }
    }
}