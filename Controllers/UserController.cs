using judo_backend.Attributes;
using judo_backend.Models;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;

namespace judo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ApiController
    {
        private IUserService _userService;
        private IEjtAdherentService _ejtAdherentService;
        private IEmailService _emailService;
        private int _secureCodeTimeout;
        private int _secureCodeLength;
        private string _secretAdherentCode;

        public static IDictionary<int, Code> _codes = new Dictionary<int, Code>();

        public UserController(ILog logger, IConfiguration configuration, IUserService userService, IEjtAdherentService ejtAdherentService, IEmailService emailService) : base(logger) 
        {
            _userService = userService;
            _emailService = emailService;
            _ejtAdherentService = ejtAdherentService;
            _secureCodeTimeout = int.Parse(configuration.GetSection("SecureCodeTimeout").Value);
            _secureCodeLength = int.Parse(configuration.GetSection("SecureCodeLength").Value);
            _secretAdherentCode = configuration.GetSection("SecretAdherentCode").Value;

        }

        [HttpPost("authenticate")]
        public ActionResult<User> Authenticate(Authenticate model)
        {
            _logger.LogInformation($"Début - Demande d'autentification pour {model.Email}");

            var user = this._userService.Authenticate(model);

            _logger.LogInformation($"Fin - Demande d'autentification pour {model.Email}");

            return Ok(user);
        }

        [HttpGet()]
        [Authorize]
        public ActionResult<List<User>> GetAll()
        {
            _logger.LogInformation($"Début - Récupération de la liste des utilisateurs");

            var users = this._userService.GetAll();

            _logger.LogInformation($"Fin - Récupération de la liste des utilisateurs");

            return Ok(users);
        }

        [HttpGet("{id}/Adherents")]
        [Authorize]
        public ActionResult<List<EjtAdherent>> GetAllAdherents(int id)
        {
            _logger.LogInformation($"Début - Récupération de la liste des adherents");

            var users = this._ejtAdherentService.GetAllByUserId(id);

            _logger.LogInformation($"Fin - Récupération de la liste des adherents");

            return Ok(users);
        }

        [HttpPost("licencesCodes/{licencesCodes}/code/{code}")]
        public ActionResult<User> Create(Authenticate model, string licencesCodes, string code) 
        {
            _logger.LogInformation($"Début - Création d'un utilisateur");

            if (code !!= this._secretAdherentCode)
            {
                return Conflict();
            }

            var licences = licencesCodes.Split(',');
            var adherents = new List<EjtAdherent>();

            foreach(var licence in licences)
            {
                var adherent = this._ejtAdherentService.Get(licence);
                
                if(adherent == null)
                {
                    return Conflict();
                }
                else
                {
                    adherents.Add(adherent);
                }
            }                

            var user = this._userService.Authenticate(model);

            if (user != null)
            {
                return Conflict();
            }

            user = this._userService.Create(model);

            foreach(var adherent in adherents)
            {
                this._userService.CreateLink(user.Id, adherent.Id);
            }

            _logger.LogInformation($"Fin - Création d'un utilisateur");

            _logger.LogInformation($"Début - Envoi du mail de bienvenue");

            this._emailService.SendWelcome(model.Email, model.Username);

            _logger.LogInformation($"Fin - Envoi du mail de bienvenue");

            return Ok(user);            
        }

        [HttpPost("{id}/licencesCodes/{licencesCodes}/code/{code}")]
        public ActionResult<List<EjtAdherent>> CreateLink(int id, string licencesCodes, string code)
        {
            _logger.LogInformation($"Début - Création d'un lien utilisateur/adhérent");

            if (code! != this._secretAdherentCode)
            {
                return Conflict();
            }

            var licences = licencesCodes.Split(',');
            var adherents = new List<EjtAdherent>();

            foreach (var licence in licences)
            {
                var adherent = this._ejtAdherentService.Get(licence);

                if (adherent == null)
                {
                    return Conflict();
                }
                else
                {
                    adherents.Add(adherent);
                }
            }

            var user = this._userService.Get(id);

            if (user == null)
            {
                return Conflict();
            }

            var actualAdherent = this._ejtAdherentService.GetAllByUserId(id);

            foreach (var adherent in adherents)
            {
                if (!actualAdherent.Any(x => x.Id == adherent.Id))
                {
                    this._userService.CreateLink(user.Id, adherent.Id);
                }
            }

            _logger.LogInformation($"Fin - Création d'un lien utilisateur/adhérent");

            return Ok(adherents);
        }

        [HttpPut()]
        [Authorize]
        public ActionResult<User> Update(User u)
        {
            _logger.LogInformation($"Début - Mise à jour d'un utilisateur");

            var user = this._userService.Update(u);

            _logger.LogInformation($"Fin - Mise à jour d'un utilisateur");

            return Ok(user);
        }

        [HttpPut("{id}/oldpassword/{oldpassword}/newpassword/{newpassword}")]
        [Authorize]
        public ActionResult<User> UpdatePassword(int id, string oldpassword, string newpassword)
        {
            _logger.LogInformation($"Début - Mise à jour du mot de pass");

            if (this._userService.CheckPassword(id, oldpassword))
            {
                var user = this._userService.UpdatePassword(id, newpassword);

                _logger.LogInformation($"Fin - Mise à jour du mot de passe");

                _logger.LogInformation($"Début - Envoi du mail de de changement de mot de passe");

                this._emailService.SendPasswordChanged(user.Email, user.Username);

                _logger.LogInformation($"Fin - Envoi du mail de de changement de mot de passe");

                return Ok(user);
            }

            _logger.LogInformation($"Fin - Mise à jour du mot de pass - Echec");

            return Conflict();
        }

        [HttpPut("{id}/code/{code}/newpassword/{newpassword}")]
        [Authorize]
        public ActionResult<User> ResetPassword(int id, string code, string newpassword)
        {
            _logger.LogInformation($"Début - Mise à jour du mot de passe");

            Code secureCode = null;
            _codes.TryGetValue(id, out secureCode);

            if (secureCode.Value == code && secureCode.Date.AddSeconds(_secureCodeTimeout) > DateTime.Now)
            {
                var user = this._userService.UpdatePassword(id, newpassword);

                _logger.LogInformation($"Fin - Mise à jour du mot de passe");

                _logger.LogInformation($"Début - Envoi du mail de de changement de mot de passe");

                this._emailService.SendPasswordChanged(user.Email, user.Username);

                _logger.LogInformation($"Fin - Envoi du mail de de changement de mot de passe");

                return Ok(user);
            }

            _logger.LogInformation($"Fin - Mise à jour du mot de passe - Echec");

            return Conflict();
        }

        [HttpPost("username/{username}")]
        public ActionResult ForgotPassword(string username)
        {
            _logger.LogInformation($"Début - Demande pour le mot de passe oublié");

            var user = this._userService.Get(username);

            if(user != null)
            {
                var code = this.GenerateCode(_secureCodeLength);

                _codes.Add(user.Id, new Code() { Date  = DateTime.Now, Value = code });

                _logger.LogInformation($"Fin - Fin pour le mot de passe oublié");

                _logger.LogInformation($"Début - Envoi du mail pour le mot de passe oublié");

                this._emailService.SendPasswordForgot(user.Id, user.Email, user.Username, user.Token, code);

                _logger.LogInformation($"Fin - Envoi du mail pour le mot de passe oublié");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult<User> Delete(int id)
        {
            _logger.LogInformation($"Début - Demande de supression d'un utilisateur");

            var user = this._userService.Get(id);

            this._userService.DeleteLink(id);
            this._userService.Delete(id);

            _logger.LogInformation($"Fin - Demande de supression d'un utilisateur");

            _logger.LogInformation($"Début - Envoi du mail GoodBye");

            this._emailService.SendGoodBye(user.Email, user.Username); 
            
            _logger.LogInformation($"Fin - Envoi du mail GoodBye");

            return Ok();
        }

        private string GenerateCode(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
