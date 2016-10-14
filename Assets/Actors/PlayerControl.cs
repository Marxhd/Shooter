﻿using UnityEngine;
using System.Collections;

public class PlayerControl : Actor
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!m_Animator.GetBool("Shooting"))
                m_Animator.SetBool("Shooting", true);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter = 0.0f;
            Plane firePlane = new Plane(Vector3.up, weaponPos.position);
            if (firePlane.Raycast(ray, out enter))
            {
                desPos = ray.GetPoint(enter);
            }
            Fire(desPos);
        }
        else
            m_Animator.SetBool("Shooting", false);
        if (Input.GetButtonUp("Fire1"))
        {
            StopFire();
        }
        // 更换武器
        float f = Input.GetAxis("Mouse ScrollWheel");
        if (f > 0)
            NextWeapon(1);
        else if (f < 0)
            NextWeapon(-1);
    }
    protected override void Die()
    {
        gameObject.SetActive(false);
        Application.Quit();
    }
    bool NextWeapon(int next)
    {
        weapons[currWeapon].gameObject.SetActive(false);
        currWeapon = Mathf.Abs(currWeapon + next + weapons.Length) % weapons.Length;
        if (weapons[currWeapon].GetComponent<Weapon>().usable)
        {
            weapons[currWeapon].gameObject.SetActive(true);
            StopFire(); // 交换武器以后，新武器是不射击状态
            return true;
        }
        else
            return NextWeapon(next);
    }
    bool ChangeWeapon(int idx)
    {
        weapons[currWeapon].gameObject.SetActive(false);
        weapons[idx].gameObject.SetActive(true);
        currWeapon = idx;
        //Debug.Log("Changed Weapon");
        return true;
    }
    public void PickupItem(GameObject item)
    {
        Item it = item.GetComponent<Item>();
        if (!it || it.tag != Tags.item)
            return;

        switch (item.GetComponent<Item>().itemType)
        {
            case ItemType.Weapon:
                PickupWeapon(item.GetComponent<Weapon>());
                break;
            case ItemType.Bullet:
                PickupBullet(item.GetComponent<Bullet>());
                break;
            case ItemType.Health:
                health += it.num;
                break;
            default:
                break;
        }
    }
    private void PickupWeapon(Weapon w)
    {
        int idx = 0;
        foreach (GameObject g in weapons)
        {
            if (g.GetComponent<Weapon>().type == w.type)
            {
                g.GetComponent<Weapon>().PickUp(w);
                g.GetComponent<Weapon>().usable = true;
                ChangeWeapon(idx);
                return;
            }
            idx++;
        }
        //Debug.Log("Weapon type error!");
    }
    private void PickupBullet(Bullet b)
    {
        int idx = 0;
        foreach (GameObject g in weapons)
        {
            if (g.GetComponent<Weapon>().type == b.bulletType)
            {
                g.GetComponent<Weapon>().PickUp(b);
                return;
            }
            idx++;
        }
    }
}
