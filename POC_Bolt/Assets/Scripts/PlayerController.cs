using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Bolt.EntityBehaviour<IPlayer> {

    public int PlayerId = 0;
    public Material[] _materials;

    private float _attackCooldDown = 2f;
    private bool _canAttack = true;

    public override void Attached()
    {
        state.SetTransforms(state.PlayerTransform, transform);
        state.SetAnimator(GetComponent<Animator>());

        PlayerId = BoltNetwork.isClient ? 1 : 0;

        Initialize();
    }

    public void Initialize()
    {
        if (entity.isOwner)
        {
            state.Color = PlayerId;
            ColorChanged();
        }

        state.AddCallback("Color", ColorChanged);
    }

    void ColorChanged()
    {

        Debug.Log(PlayerId);

        Material material = _materials[state.Color];

        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = material;
        }
    }

    public override void SimulateOwner()
    {
        var speed = 0f;
        var movementValue = 10f;
        var rotationSpeed = 180f;
        var rotation = Vector3.zero;

        if (Input.GetKey(KeyCode.Z)) { speed += movementValue; }
        if (Input.GetKey(KeyCode.S)) { speed -= movementValue; }
        if (Input.GetKey(KeyCode.Q)) { rotation.y -= 1; }
        if (Input.GetKey(KeyCode.D)) { rotation.y += 1; }

        if (entity.isOwner && _canAttack) { 
            if (Input.GetKeyDown(KeyCode.Space)) {
                _canAttack = false;
                state.IsAttacking = true;
                var updateScore = PlayerScoring.Create();
                updateScore.PlayerIndex = PlayerId;
                updateScore.Send();
                StartCoroutine(CancelIsAttacking());
            }
        }

        if (speed != 0)
        {
            transform.position = transform.position + (transform.forward * speed * BoltNetwork.frameDeltaTime);
        }

        if (rotation != Vector3.zero) {
            transform.Rotate(rotation * rotationSpeed * BoltNetwork.frameDeltaTime);
        }
    }

    IEnumerator CancelIsAttacking()
    {
        float currentTime = 0f;
        yield return new WaitForSeconds(0.1f);

        currentTime += BoltNetwork.frameDeltaTime;

        if (entity.isOwner)
        {
            state.IsAttacking = false;
        }

        while (currentTime < _attackCooldDown)
        {
            yield return null;
            currentTime += BoltNetwork.frameDeltaTime;
        }

        _canAttack = true;
    }
}
 