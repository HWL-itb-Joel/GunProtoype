using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    [Header("Guns Selected")]
    private Gun weaponInfo;
    public Gun primaryWeapon;
    private GameObject primaryObject;
    public Gun secundayWeapon;
    private GameObject secundayObject;
    public MeleeWeapon meleeWeapon;
    public ThrowableWeapon throwableWeapon1;
    public ThrowableWeapon throwableWeapon2;

    [Header("Input Settings")]
    public InputActionAsset PlayerInputs;
    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction lookAction;
    private InputAction scrollWeapons;

    private Vector2 lookInput;
    private float scroll;

    [Header("Gun Settings")]
    public int _currentAmmoInClip;
    public int _ammoInReserve;
    public bool _canShoot;
    private bool shootEnded;
    private bool _gunEnabled;
    private bool firstThroweableEnabled;

    public int _currentAmmoInClipReserve;
    public int _ammoInReserveReserve;

    [Header("Mouse Settings")]
    public float mouseSensitiity = 1;
    Vector2 _currentRotation;

    [Header("Sway Settings")]
    public float swayAmount = 0.02f;
    public float maxSway = 0.1f;
    public float smoothSpeed = 6f;

    private Vector3 initialPosition;

    [Header("Sway Settings")]
    public bool randomizeRecoil;
    public Vector2 randomRecoilConstrains;
    //pattern recoil
    public Vector2 recoilPattern;

    private void Awake()
    {
        weaponInfo = primaryWeapon;
        primaryObject = Instantiate(primaryWeapon.prefab, gameObject.transform);
        primaryObject.SetActive(true);
        secundayObject = Instantiate(secundayWeapon.prefab,gameObject.transform);
        secundayObject.SetActive(false);
        scrollWeapons = PlayerInputs.FindActionMap("OnGround").FindAction("PrimaryWeapon");
        fireAction = PlayerInputs.FindActionMap("OnGround").FindAction("Fire");
        reloadAction = PlayerInputs.FindActionMap("OnGround").FindAction("Reload");
        lookAction = PlayerInputs.FindActionMap("OnGround").FindAction("Look");

        lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => lookInput = Vector2.zero;

        scrollWeapons.performed += context => scroll = context.ReadValue<float>();
    }

    private void OnEnable()
    {
        scrollWeapons.Enable();
        fireAction.Enable();
        reloadAction.Enable();
        lookAction.Enable();
    }

    private void OnDisable()
    {
        scrollWeapons.Disable();
        fireAction.Disable();
        reloadAction.Disable();
        lookAction.Disable();
    }

    private void Start()
    {
        initialPosition = transform.localPosition;
        _currentAmmoInClip = weaponInfo.clipSize;
        _ammoInReserve = weaponInfo.reservedAmmoCapacity;
        _currentAmmoInClipReserve = secundayWeapon.clipSize;
        _ammoInReserveReserve = secundayWeapon.reservedAmmoCapacity;
        _canShoot = true;
        firstThroweableEnabled = true;
        _gunEnabled = true;
    }

    private void Update()
    {
        DetermineRotation();
        if (fireAction.IsPressed())
        {
            if (_canShoot && _currentAmmoInClip > 0 && _gunEnabled)
            {
                StartCoroutine(ShootGun());
            }
            else if (!_gunEnabled && weaponInfo.weaponType == WeaponType.Melee)
            {
                //Desarrollo melee
            }
            else if (!_gunEnabled && weaponInfo.weaponType == WeaponType.Arrojadiza)
            {
                //arrojadiza
                if (firstThroweableEnabled)
                {

                }
                else
                {

                }
            }
        }
        else if (weaponInfo.rateType == fireRateType.OneShot && !fireAction.IsPressed() && shootEnded && _gunEnabled)
        {
            _canShoot = true;
        }

        if (reloadAction.IsPressed() && _currentAmmoInClip < weaponInfo.clipSize && _ammoInReserve > 0 && weaponInfo.weaponType != WeaponType.Secundaria)
        {
            Reload();
        }
        else if (reloadAction.IsPressed() && _currentAmmoInClip < weaponInfo.clipSize && weaponInfo.weaponType == WeaponType.Secundaria)
        {
            _currentAmmoInClip = weaponInfo.clipSize;
        }
        if (scroll != 0)
        {
            ChangeGunWeapon();
        }
    }

    void DetermineRotation()
    {
        Vector2 mouseAxis = new Vector2(lookInput.x, -lookInput.y);

        mouseAxis *= mouseSensitiity;
        _currentRotation += mouseAxis;

        _currentRotation.y = Mathf.Clamp(_currentRotation.y, -90, 90);

        float mouseX = lookInput.x * swayAmount;
        float mouseY = lookInput.y * swayAmount;

        mouseX = Mathf.Clamp(mouseX, -maxSway, maxSway);
        mouseY = Mathf.Clamp(mouseY, -maxSway, maxSway);

        Vector3 targetPosition = new Vector3(initialPosition.x - mouseX, initialPosition.y - mouseY, initialPosition.z);

        // Interpolar suavemente la posición del arma
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);

        transform.root.localRotation = Quaternion.AngleAxis(_currentRotation.x, Vector3.up);
        transform.parent.localRotation = Quaternion.AngleAxis(_currentRotation.y, Vector3.right);
    }

    void DetermineRecoil()
    {
        transform.localPosition -= Vector3.forward * 0.075f;

        if (randomizeRecoil)
        {
            float xRecoil = Random.Range(-randomRecoilConstrains.x, randomRecoilConstrains.x);
            float yRecoil = Random.Range(-randomRecoilConstrains.y, randomRecoilConstrains.y);

            Vector2 recoild = new Vector2(xRecoil, yRecoil);

            _currentRotation += recoild;
        }
    }

    IEnumerator ShootGun()
    {
        if (weaponInfo.rateType == fireRateType.Auto)
        {
            _canShoot = false;
            _currentAmmoInClip--;
            DetermineRecoil();
            RatCastForEnemy();
            yield return new WaitForSeconds(weaponInfo.fireRate);
            _canShoot = true;
        }
        else if (weaponInfo.rateType == fireRateType.Semi)
        {
            _canShoot = false;
            for (int i = 0; i < weaponInfo.bulletsByShot; i++)
            {
                _currentAmmoInClip--;
                DetermineRecoil();
                RatCastForEnemy();
                yield return new WaitForSeconds(weaponInfo.bulletsFireRate);
            }
            yield return new WaitForSeconds(weaponInfo.fireRate);
            _canShoot = true;
        }
        else if (weaponInfo.rateType == fireRateType.OneShot)
        {
            _canShoot = false;
            shootEnded = false;
            _currentAmmoInClip -= weaponInfo.bulletsByShot;
            DetermineRecoil();
            RatCastForEnemy();
            yield return new WaitForSeconds(weaponInfo.fireRate);
            shootEnded = true;
        }
    }

    void RatCastForEnemy()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, weaponInfo.maxDistance))
        {
            Debug.DrawRay(transform.parent.position, hit.point, Color.red, 834485f);
            try
            {
                Debug.Log("Hit an enemy");
            }
            catch { }
        }
    }

    void ChangeGunWeapon()
    {
        int ammoInClip = _currentAmmoInClip;
        int ammoReserve = _ammoInReserve;

        if (weaponInfo == primaryWeapon)
        {
            weaponInfo = secundayWeapon;
            primaryObject.SetActive(false);
            secundayObject.SetActive(true);

        }
        else if (weaponInfo == secundayWeapon)
        {
            weaponInfo = primaryWeapon;
            primaryObject.SetActive(true);
            secundayObject.SetActive(false);
        }

        _ammoInReserve = _ammoInReserveReserve;
        _currentAmmoInClip = _currentAmmoInClipReserve;

        _ammoInReserveReserve = ammoReserve;
        _currentAmmoInClipReserve = ammoInClip;
    }

    public void Reload()
    {
        int amountNeeded = weaponInfo.clipSize - _currentAmmoInClip;
        if (amountNeeded >= _ammoInReserve)
        {
            _currentAmmoInClip += _ammoInReserve;
            _ammoInReserve -= amountNeeded;
        }
        else
        {
            _currentAmmoInClip = weaponInfo.clipSize;
            _ammoInReserve -= amountNeeded;
        }
    }
}
