namespace PassphraseManagerSvc.Dto
{
    public class ApiResponeDto<Dto>
    {
        public Dto Result { get; set; }
        public bool HasError { get; set; }

        public Error ErrorDetails { get; set; }
    }
}