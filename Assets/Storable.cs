namespace ACE2EU {

    public enum StorableType {
        FireWood,
        FreeRide,
    }

    public class Storable: Interactable {
        public StorableType Type = (StorableType)(-1);

        public override void Interact() {
            Inventory.Instance.Add(this);
        }
    }
}
