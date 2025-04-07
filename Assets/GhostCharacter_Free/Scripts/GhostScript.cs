using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    public class GhostScript : MonoBehaviour
    {
        private Animator Anim;
        private CharacterController Ctrl;
        private Vector3 MoveDirection = Vector3.zero;

        private static readonly int IdleState = Animator.StringToHash("Base Layer.idle");
        private static readonly int MoveState = Animator.StringToHash("Base Layer.move");
        private static readonly int SurprisedState = Animator.StringToHash("Base Layer.surprised");
        private static readonly int AttackState = Animator.StringToHash("Base Layer.attack_shift");
        private static readonly int DissolveState = Animator.StringToHash("Base Layer.dissolve");
        private static readonly int AttackTag = Animator.StringToHash("Attack");

        [SerializeField] private SkinnedMeshRenderer[] MeshR;
        private float Dissolve_value = 1;
        private bool DissolveFlg = false;
        private const int maxHP = 3;
        private int HP = maxHP;
        private Text HP_text;

        [SerializeField] private float Speed = 4;
        [SerializeField] private float JumpForce = 5f;


        void Start()
        {
            Anim = this.GetComponent<Animator>();
            Ctrl = this.GetComponent<CharacterController>();
            HP_text = GameObject.Find("Canvas/HP").GetComponent<Text>();
            HP_text.text = "HP " + HP.ToString();
        }

        void Update()
        {
            STATUS();
            GRAVITY();
            Respawn();

            if (!PlayerStatus.ContainsValue(true))
            {
                MOVE();
                PlayerAttack();
                Damage();
            }
            else if (PlayerStatus.ContainsValue(true))
            {
                int status_name = 0;
                foreach (var i in PlayerStatus)
                {
                    if (i.Value == true)
                    {
                        status_name = i.Key;
                        break;
                    }
                }
                if (status_name == Dissolve)
                {
                    PlayerDissolve();
                }
                else if (status_name == Attack)
                {
                    PlayerAttack();
                }
                else if (status_name == Surprised)
                {
                    // nothing method
                }
            }

            if (HP <= 0 && !DissolveFlg)
            {
                Anim.CrossFade(DissolveState, 0.1f, 0, 0);
                DissolveFlg = true;
            }
            else if (HP == maxHP && DissolveFlg)
            {
                DissolveFlg = false;
            }
        }

        private const int Dissolve = 1;
        private const int Attack = 2;
        private const int Surprised = 3;
        private Dictionary<int, bool> PlayerStatus = new Dictionary<int, bool>
    {
        {Dissolve, false },
        {Attack, false },
        {Surprised, false },
    };

        private void STATUS()
        {
            PlayerStatus[Dissolve] = DissolveFlg && HP <= 0;
            PlayerStatus[Attack] = Anim.GetCurrentAnimatorStateInfo(0).tagHash == AttackTag;
            PlayerStatus[Surprised] = Anim.GetCurrentAnimatorStateInfo(0).fullPathHash == SurprisedState;
        }

        private void PlayerDissolve()
        {
            Dissolve_value -= Time.deltaTime;
            foreach (var mesh in MeshR)
            {
                mesh.material.SetFloat("_Dissolve", Dissolve_value);
            }
            if (Dissolve_value <= 0)
            {
                Ctrl.enabled = false;
            }
        }

        private void PlayerAttack()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Anim.CrossFade(AttackState, 0.1f, 0, 0);
            }
        }

        private void GRAVITY()
        {
            if (Ctrl.enabled)
            {
                if (CheckGrounded())
                {
                    if (Input.GetKeyDown(KeyCode.Space)) // salto
                    {
                        MoveDirection.y = JumpForce;
                    }
                    else if (MoveDirection.y < -0.1f)
                    {
                        MoveDirection.y = -0.1f;
                    }
                }

                MoveDirection.y -= 9.81f * Time.deltaTime; // gravedad más realista
                Ctrl.Move(MoveDirection * Time.deltaTime);
            }
        }


        private bool CheckGrounded()
        {
            if (Ctrl.isGrounded && Ctrl.enabled) return true;
            Ray ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
            return Physics.Raycast(ray, 0.2f);
        }

        private void MOVE()
        {
            if (Anim.GetCurrentAnimatorStateInfo(0).fullPathHash == MoveState)
            {
                if (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                {
                    MOVE_Velocity(new Vector3(0, 0, Speed), new Vector3(0, 0, 0));
                }
                else if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                {
                    MOVE_Velocity(new Vector3(0, 0, -Speed), new Vector3(0, 180, 0));
                }
                else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow))
                {
                    MOVE_Velocity(new Vector3(-Speed, 0, 0), new Vector3(0, 270, 0));
                }
                else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow))
                {
                    MOVE_Velocity(new Vector3(Speed, 0, 0), new Vector3(0, 90, 0));
                }
            }

            KEY_DOWN();
            KEY_UP();
        }

        private void MOVE_Velocity(Vector3 velocity, Vector3 rot)
        {
            MoveDirection = new Vector3(velocity.x, MoveDirection.y, velocity.z);
            if (Ctrl.enabled)
            {
                Ctrl.Move(MoveDirection * Time.deltaTime);
            }
            MoveDirection.x = 0;
            MoveDirection.z = 0;
            this.transform.rotation = Quaternion.Euler(rot);
        }

        private void KEY_DOWN()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Anim.CrossFade(MoveState, 0.1f, 0, 0);
            }
        }

        private void KEY_UP()
        {
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) ||
                Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) &&
                    !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                {
                    Anim.CrossFade(IdleState, 0.1f, 0, 0);
                }
            }
        }

        private void Damage()
        {
            if (Input.GetKeyUp(KeyCode.S))
            {
                Anim.CrossFade(SurprisedState, 0.1f, 0, 0);
                HP--;
                HP_text.text = "HP " + HP.ToString();
            }
        }

        private void Respawn()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                HP = maxHP;
                Ctrl.enabled = false;
                this.transform.position = Vector3.zero;
                this.transform.rotation = Quaternion.Euler(Vector3.zero);
                Ctrl.enabled = true;

                Dissolve_value = 1;
                foreach (var mesh in MeshR)
                {
                    mesh.material.SetFloat("_Dissolve", Dissolve_value);
                }

                Anim.CrossFade(IdleState, 0.1f, 0, 0);
            }
        }
    }
}
