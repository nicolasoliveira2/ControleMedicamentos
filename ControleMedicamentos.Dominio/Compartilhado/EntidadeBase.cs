namespace ControleMedicamentos.Dominio
{
    public abstract class EntidadeBase<T>
    {

        public int Numero { get; set; }

        public override bool Equals(object obj)
        {
            return obj is EntidadeBase<T> @base &&
                   Numero == @base.Numero;
        }
    }
}
