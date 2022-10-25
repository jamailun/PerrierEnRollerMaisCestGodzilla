using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy {

    [Header("Boss data")]

    [SerializeField] private CustomAnimation aoeAnimation;
    [SerializeField] private CustomAnimation flyAnimation;
    [SerializeField] private CustomAnimation deathAnimation;

    [Header("Intro")]
    [SerializeField] private bool passIntroDebug = false;
    [SerializeField] private float introZoomSpeed = 2f;
    [SerializeField] private float introDuration = 1f;
    [SerializeField] private AudioClip introSfx;
    [SerializeField] private TMPro.TMP_Text bossNameText;

    [Header("Parameters")]
    [SerializeField] private float recalculatePath = 0.4f;
    [SerializeField] private float maxToGoBackToDistance = 11f;
    [SerializeField] private float loadingAoE = 0.6f;
    [SerializeField] private float bossMeleeAttackDuration = 1.2f;
    [SerializeField] private float newPhaseMin = 3f;
    [SerializeField] private float newPhaseMax = 5f;

    [Header("Attack elements")]
    [SerializeField] private AudioClip meleeClip;
    [SerializeField] private DamageZone damageZone;
    [SerializeField] private SkillShot aoePrefab;
    [SerializeField] private DamageZone laserPrefab;
    [SerializeField] private float laserPrefabSpacing = 24f;

    private float nextPathCalcul;
    private float endIntro;

    private enum Phase {
        Intro,

        DistanceRushing,
        CaC,
        AoE,
        AwaySkillShot_anim,
        AwaySkillShot,

        Dead
	}

    private Phase phase;
    private bool ignoreRecalculate = true;

    private const string ANIM_FLY = "anim_fly";
    private const string ANIM_AOE = "anim_aoe";
    private const string ANIM_DEATH = "anim_death";

    protected override void Start() {
        phase = Phase.Intro;

        agent = GetComponent<NavMeshAgent>();
        if(agent == null) {
            Debug.LogWarning("Warning enemy " + name + "should have a NavMeshAgent component.");
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // laisse le temps de faire des trucs xd
        StartCoroutine(Utils.DoAfter(1f, () => {
            if(passIntroDebug) {
                RealStart();
            } else {
                StartCoroutine(PlayIntro());
            }
        }));
    }

    private IEnumerator PlayIntro() {
        if(LevelMusicPlayer.CurrentInstance != null)
            LevelMusicPlayer.CurrentInstance.Stop();

        // 1 zoom caméra
        ManagerUI.Instance.HideAllNonBoss();

        var cam = Camera.main;
        float originalSize = cam.orthographicSize;
        var cf = cam.GetComponent<CameraFollow>();
        Transform playerTr = cf.target;
        if(cf)
            cf.target = transform;

        if(introSfx) {
            var obj = new GameObject("sfx_boss_" + name);
            obj.transform.position = transform.position;
            var audio = obj.AddComponent<AudioSource>();
            audio.PlayOneShot(introSfx);
            Destroy(obj, introSfx.length);
        }

        while(cam.orthographicSize >= 5f) {
            cam.orthographicSize -= Time.deltaTime * introZoomSpeed;

            if(cam.orthographicSize <= 8.5f && bossNameText!=null && ! bossNameText.gameObject.activeSelf)
                bossNameText.gameObject.SetActive(true);

            yield return null;
		}

        yield return new WaitForSeconds(introDuration);

        if(bossNameText != null)
           bossNameText.gameObject.SetActive(false);
        ManagerUI.Instance.ShowAllNonBoss();

        cam.orthographicSize = originalSize;
        if(cf)
            cf.target = playerTr;

        RealStart();
	}

    private void RealStart() {
        ignoreRecalculate = false;

        if(healthBar != null)
            healthBar.gameObject.SetActive(true);

        base.Start();

        animator.SetClip(ANIM_FLY, flyAnimation);
        animator.SetClip(ANIM_AOE, aoeAnimation);
        animator.SetClip(ANIM_DEATH, deathAnimation);

        if(LevelMusicPlayer.CurrentInstance != null)
            LevelMusicPlayer.CurrentInstance.PlayBossMusic();

        agent.isStopped = false;
    }

    private Vector3 Dir(Vector3 a, Vector3 b) {
        return new Vector3(b.x - a.x, b.y - a.y).normalized;
	}

    private bool lastProjLeft = false;
    private float nextPhase;
	protected override void Recalculate() {

        if(ignoreRecalculate)
            return;

        if(Time.time >= nextPathCalcul) {
            nextPathCalcul = Time.time + recalculatePath;
            agent.SetDestination(Target.position);
        }

        switch(phase) {
        // useless
            case Phase.Intro:
                if(Time.time >= endIntro) {
                    RefreshTarget();
                    agent.isStopped = false;
                    phase = agent.remainingDistance > distance_wanted ? Phase.DistanceRushing : Phase.CaC;
				}
                break;
        // \\
            case Phase.DistanceRushing:
                // Try to attack
                if(Time.time >= nextAttackAllowed) {
                    BossAttackDistance();
                    break;
                }
                // Try to switch to CàC
                if(agent.remainingDistance <= distance_wanted) {
                    phase = Phase.CaC;
                    RecalculateNextPhase();
                }
                break;

            case Phase.CaC:

                // Attack càc
                if(Time.time >= nextAttackAllowed) {
                    BossAttackCaC();
                    break;
                }

                if(Time.time >= nextPhase) {
                    float r = Random.Range(0f, 1f);
                    if(r <= 0.25f) {
                        // do nothing
                        RecalculateNextPhase();
					} else {
                        phase = Phase.AoE;
                        aoeAttack = Time.time + loadingAoE;
                        agent.isStopped = true;
                        nAoE = 3;
                    }
                } else if(agent.remainingDistance > maxToGoBackToDistance) {
                    phase = Phase.DistanceRushing;
                    RecalculateNextPhase();
                }
                break;

            case Phase.AoE:
                if(Time.time >= aoeAttack) {
                    BossAttackAoE();
                    nAoE--;
                    float loading = Random.Range(1f, 1.5f);
                    animator.PlayOnce(ANIM_AOE, loading, ANIM_WALK);
                    aoeAttack = Time.time + loading;

                    if(nAoE <= 0) {
                        phase = Phase.DistanceRushing;
                        agent.isStopped = false;
                        RecalculateNextPhase();
                    }
				}
                break;

            case Phase.AwaySkillShot_anim:
                phase = Phase.DistanceRushing;
                break;
        }
    }
    private int nAoE;
    private float aoeAttack;

    private void BossAttackDistance() {
        nextAttackAllowed = Time.time + (GetAttackSpeed() * (1f + Random.Range(-attackSpeedEpsilon, attackSpeedEpsilon)));
        proj_n = 0;

        // direction
        var dir = Dir(GetOutput(), Target.position);
        var proj_dir = Quaternion.AngleAxis(lastProjLeft ? -90f : 90f, Vector3.forward) * dir;
        lastProjLeft = !lastProjLeft;

        // SFX
        AttackEffect(attack_output.position);

        // Projectil
        var proj = Instantiate(projectile_prefab, new(attack_output.position.x, attack_output.position.y, 0.1f), Quaternion.identity);
        proj.transform.localScale = transform.localScale / 2f;
        proj.GetComponent<Rigidbody2D>().AddForce(proj_dir * 8f);
        if(proj.GetType() == typeof(FollowProjectile)) {
            ((FollowProjectile) proj).InitFollow(Target, transform);
        }

        if(agent.remainingDistance <= distance_wanted) {
            phase = Phase.CaC;
            nextPhase = Time.time + Random.Range(newPhaseMin, newPhaseMax);
        }
    }

    private void BossAttackAoE() {
        nextAttackAllowed = Time.time + (GetAttackSpeed() * (1f + Random.Range(-attackSpeedEpsilon, attackSpeedEpsilon)));

        var aoe = Instantiate(aoePrefab, Target.position, Quaternion.identity);
       // aoe.transform.localScale = transform.localScale / 2.5f;
        aoe.Init(_flatDamages, transform.localScale.x);
    }

    private float GetAttackSpeed() {
        return attackSpeed;
	}

    private bool attacking = false;
    private void BossAttackCaC() {
        if(attacking)
            return;
        nextAttackAllowed = Time.time + (GetAttackSpeed() * (1f + Random.Range(-attackSpeedEpsilon, attackSpeedEpsilon)));

        nextAttackAllowed += bossMeleeAttackDuration;

        var hitbox = Instantiate(damageZone, attack_output);
        hitbox.transform.localPosition = new Vector3(0, 0, 0);
        // hitbox.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        /* foreach(var ps in hitbox.GetComponentsInChildren<ParticleSystem>()) {
             var sh = ps.shape;
             sh.scale = transform.localScale / 2f;
         }*/

        gameObject.GetOrAddComponent<AudioSource>().PlayOneShot(meleeClip);

        animator.Play(ANIM_ATTACK);
        agent.speed /= 2f;
        attacking = true;

        StartCoroutine(Utils.DoAfter(bossMeleeAttackDuration, () => {
            animator.Play(ANIM_WALK);
            attacking = false;
            agent.speed *= 2f;
            Destroy(hitbox.gameObject);
        }));
    }

    private void RecalculateNextPhase() {
        nextPhase = Time.time + Random.Range(newPhaseMin, newPhaseMax);
    }
    protected override void Die() {
        // WIN THE GAME
        agent.isStopped = true;
        phase = Phase.Dead;
        dead = true;

        var player = Target.GetComponent<PlayerEntity>();
        if(player == null)
            player = FindObjectOfType<PlayerEntity>();
        if(player != null) {
            player.MakeInvicible();
            TimerUI.Stop();
            PersistentData.EndRun(player.TimeSinceStart, player.Level, player.UpgradePoints);
        }

        Debug.Log("FIN DU JEU BRAVOOOO");

        // death
        animator.PlayOnce(ANIM_DEATH, 2f);
        StartCoroutine(Utils.DoAfter(1f, () => {
            animator.Stop();
        }));
        LevelMusicPlayer.CurrentInstance.Stop();
        StartCoroutine(Utils.DoAfter(3f, () => {
            UnityEngine.SceneManagement.SceneManager.LoadScene("WinGameScene");
        }));
	}

    public void SpawnLasersHorizontal(float y, float width, float duration) {
        float x = 0;
        while(x <= width) {
            var laser = Instantiate(laserPrefab, new Vector3(x, y), Quaternion.identity);
            Utils.DestroyAfter(laser.gameObject, duration);
            x += laserPrefabSpacing;
        }
	}

    public override void MakeAfraid(float duration) {
        base.MakeAfraid(duration / 3f);
	}

}