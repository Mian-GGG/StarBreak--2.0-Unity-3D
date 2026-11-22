public interface IWeapon
{
    void Shoot();
    void Reload();
    void Aim(bool isAiming);
    string WeaponName { get; }
}