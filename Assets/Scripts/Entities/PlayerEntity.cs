using UnityEngine;

public class PlayerEntity : LivingEntity {

    // TODO plein de trucs pour le joueur.

    // Moyen le plus rapide de savoir si une entité est joueur ou pas, plutot que de la réflexion à la con.
    public override bool IsPlayer() {
        return false;
    }

    public float GetSpeed() {
        // Can be used to do slow/run effects.
        return _speed;
	}

}
