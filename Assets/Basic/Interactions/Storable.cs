namespace ACE2EU {

    public enum StorableType {
        FireWood,
        FreeRide,
        Sausage,
        BavarianHat,
        BeerMug,
        Car,
        Note,
        Medicine
    }

    public class Storable: Interactable {
        public StorableType Type = (StorableType)(-1);

        public override void Interact() {
            Inventory.Instance.Add(this);
        }
    }
}
