namespace AspHttpSecurity.data
{
    public class Allreqs
    {
        private RequestAtributes atribute;

        public Allreqs(RequestAtributes atribute)
        {
            this.atribute = atribute;
        }

        public void autenticate()
        {
            atribute.autenticate = true;
        }

        public void permitAll()
        {
            atribute.autenticate = false;
        }
    }
}
