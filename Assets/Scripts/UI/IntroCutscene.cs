using System.Collections;
using UnityEngine;
using TMPro;

public class IntroCutscene : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public CameraFollow cameraFollow;

    public Transform playerTransform;
    public Transform elaraTransform;
    public Animator elaraAnimator;

    public GameObject morvathObject;
    public Transform morvathTransform;

    public GameObject basketObject;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI objectiveText;

    [Header("Points")]
    public Transform elaraWalkPoint;
    public Transform morvathAppearPoint;
    public Transform morvathEscapePoint;
    public Transform kaelRunPoint;

    [Header("Timing")]
    public float dialogueDuration = 2f;
    public float shortPause = 0.25f;
    public float elaraWalkDuration = 2.2f;
    public float morvathApproachDuration = 0.45f;
    public float kidnapEscapeDuration = 1.2f;
    public float kaelRunDuration = 1.2f;

    private void Start()
    {
        StartCoroutine(Cutscene());
    }

    private IEnumerator Cutscene()
    {
        // Başlangıç
        if (playerMovement != null)
            playerMovement.canMove = false;

        if (dialogueText != null)
            dialogueText.text = "";

        if (objectiveText != null)
            objectiveText.gameObject.SetActive(false);

        if (basketObject != null)
            basketObject.SetActive(false);

        if (morvathObject != null)
            morvathObject.SetActive(false);

        // Başta kamera Player'da kalsın ki ikisini de görsün
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(playerTransform);
            cameraFollow.SnapToTarget();
        }

        // Başta birbirlerine baksınlar
        LookAtNormal(playerTransform, elaraTransform);
        LookAtElara(elaraTransform, playerTransform);

        // SAHNE 1
        yield return Say("Elara: Yiyecekler azalmış. Ormandan biraz bir şey toplayıp hemen döneceğim.");
        yield return Say("Kael: Şimdi mi? Hava kararmaya başlıyor.");
        yield return Say("Elara: Uzaklaşmayacağım. Merak etme.");

        // SAHNE 2
        yield return Say("Kael: Bugün orman farklı hissettiriyor... dikkatli ol.");
        yield return Say("Elara: Çok sürmez. Döndüğümde ateşi yakmış ol.");

        // Elara yürümeye başlayınca kamera Elara'yı takip etsin
        if (cameraFollow != null)
            cameraFollow.SetTarget(elaraTransform);

        FaceRightElara(elaraTransform);

        if (elaraAnimator != null)
            elaraAnimator.SetBool("IsWalking", true);

        yield return Move(elaraTransform, elaraWalkPoint.position, elaraWalkDuration);

        if (elaraAnimator != null)
            elaraAnimator.SetBool("IsWalking", false);

        // SAHNE 3
        yield return Say("Kael: İçime sinmiyor...");

        // SAHNE 4
        yield return Say("Elara: Kim var orada?");
        yield return new WaitForSeconds(0.6f);

        // SAHNE 5 - Morvath görünür
        if (morvathObject != null)
        {
            morvathObject.SetActive(true);

            if (morvathAppearPoint != null && morvathTransform != null)
                morvathTransform.position = morvathAppearPoint.position;
        }

        FaceRightElara(elaraTransform);

        yield return Say("Morvath: ...Sen...");
        yield return Say("Elara: Çık ortaya!");
        yield return Say("Morvath: Bu kez seni kaybetmeyeceğim.");
        yield return Say("Elara: Beni biriyle karıştırıyorsun... yaklaşma!");

        // SAHNE 6 - Kaçırılma
        if (morvathTransform != null && elaraTransform != null)
        {
            Vector3 grabPoint = new Vector3(
                elaraTransform.position.x + 0.18f,
                elaraTransform.position.y,
                elaraTransform.position.z
            );

            yield return Move(morvathTransform, grabPoint, morvathApproachDuration);
        }

        yield return Say("Elara: Kael!");

        // Sepet düşsün
        if (basketObject != null && elaraTransform != null)
        {
            basketObject.SetActive(true);
            basketObject.transform.position = new Vector3(
                elaraTransform.position.x - 0.18f,
                elaraTransform.position.y - 0.20f,
                0f
            );
        }

        // Elara + Morvath sağa kaçsın
        FaceRightElara(elaraTransform);

        if (elaraAnimator != null)
            elaraAnimator.SetBool("IsWalking", true);

        yield return MovePairTight(elaraTransform, morvathTransform, morvathEscapePoint.position, kidnapEscapeDuration);

        if (elaraAnimator != null)
            elaraAnimator.SetBool("IsWalking", false);

        // Kaçırıldıktan sonra Elara yok olsun
        if (elaraTransform != null)
            elaraTransform.gameObject.SetActive(false);

        if (morvathObject != null)
            morvathObject.SetActive(false);

        // SAHNE 7 - Sessizlik, kamera biraz sepette/boşlukta kalsın
        yield return new WaitForSeconds(1.2f);

        // SAHNE 8 - Kamera Player'a dönsün
        if (cameraFollow != null)
            cameraFollow.SetTarget(playerTransform);

        // Kael sepete doğru koşsun
        FaceRightNormal(playerTransform);
        yield return Move(playerTransform, kaelRunPoint.position, kaelRunDuration);

        yield return Say("Kael: Elara?!");
        yield return new WaitForSeconds(0.4f);
        yield return Say("Kael: Hayır...");
        yield return Say("Kael: Seni bulacağım. Ne pahasına olursa olsun.");

        // SAHNE 10 - Objective + kontrol
        if (objectiveText != null)
        {
            objectiveText.gameObject.SetActive(true);
            objectiveText.text = "Elara'nın izlerini takip et";
        }

        if (playerMovement != null)
            playerMovement.canMove = true;
    }

    private IEnumerator Say(string text)
    {
        if (dialogueText != null)
            dialogueText.text = text;

        yield return new WaitForSeconds(dialogueDuration);
        yield return new WaitForSeconds(shortPause);
    }

    private IEnumerator Move(Transform obj, Vector3 target, float duration)
    {
        Vector3 start = obj.position;
        float t = 0f;

        while (t < duration)
        {
            obj.position = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        obj.position = target;
    }

    private IEnumerator MovePairTight(Transform elara, Transform morvath, Vector3 morvathTarget, float duration)
    {
        Vector3 morvathStart = morvath.position;
        Vector3 tightOffset = new Vector3(0.16f, 0f, 0f);

        float t = 0f;

        while (t < duration)
        {
            morvath.position = Vector3.Lerp(morvathStart, morvathTarget, t / duration);
            elara.position = morvath.position + tightOffset;

            t += Time.deltaTime;
            yield return null;
        }

        morvath.position = morvathTarget;
        elara.position = morvath.position + tightOffset;
    }

    private void LookAtNormal(Transform from, Transform to)
    {
        if (from == null || to == null) return;

        SpriteRenderer sr = from.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return;

        sr.flipX = to.position.x < from.position.x;
    }

    private void FaceRightNormal(Transform target)
    {
        if (target == null) return;

        SpriteRenderer sr = target.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return;

        sr.flipX = false;
    }

    private void LookAtElara(Transform from, Transform to)
    {
        if (from == null || to == null) return;

        SpriteRenderer sr = from.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return;

        sr.flipX = to.position.x > from.position.x;
    }

    private void FaceRightElara(Transform target)
    {
        if (target == null) return;

        SpriteRenderer sr = target.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return;

        sr.flipX = true;
    }
}