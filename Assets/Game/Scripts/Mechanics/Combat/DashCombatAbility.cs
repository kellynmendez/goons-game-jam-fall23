using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCombatAbility : ICombatAbility
{
    private MonoBehaviour _mono;
    private PlayerController _player;
    private CharacterController _controller;
    private float _dashSpeed;
    private float _dashDuration;
    private float _dashCooldown;
    private bool _dashIsCoolingDown = false;

    private float _currDashSpeed;

    public DashCombatAbility(PlayerController player, CharacterController controller, float dashSpeed, 
        float dashDuration, float dashCooldown)
    {
        _mono = player as MonoBehaviour;
        _player = player;
        _controller = controller;
        _dashSpeed = _currDashSpeed = dashSpeed; 
        _dashDuration = dashDuration;
        _dashCooldown = dashCooldown;
    }

    public void UseAbility()
    {
        if (!_dashIsCoolingDown)
        {
            _mono.StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        float startTime = Time.time;
        Vector3 dashDirection = _controller.transform.forward;
        _dashIsCoolingDown = true;
        _player.IsUsingGoonAbility = true;
        float tempTime = 0;
        // Dashing
        while (Time.time < startTime + _dashDuration)
        {
            tempTime += Time.deltaTime;
            if (tempTime > 0.1f)
            {
                tempTime = 0;
                _currDashSpeed *= 0.9f;
            }
            _controller.Move(dashDirection * _currDashSpeed * Time.deltaTime);
            yield return null;
        }
        _currDashSpeed = _dashSpeed;
        _player.IsUsingGoonAbility = false;

        // Dash cooldown
        yield return new WaitForSeconds(_dashCooldown);
        _dashIsCoolingDown = false;
    }
}
