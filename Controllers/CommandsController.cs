using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        { 
            _repository = repository;
            _mapper = mapper;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Getting Commands for platform: {platformId} from Commands Service");

            if(!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }
            var commandsForPlatform = _repository.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandsForPlatform));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Getting Command {commandId} for platform: {platformId} from Commands Service");

            if(!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }
            var commandForPlatform = _repository.GetCommand(platformId, commandId);
            if(commandForPlatform == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(commandForPlatform));
        }


        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Creating Command for platform: {platformId} from Commands Service");

            if(!_repository.PlatformExists(platformId))
            {
                return NotFound();
            } 
            
            var commandEntity = _mapper.Map<Command>(commandDto);
            _repository.CreateCommand(platformId, commandEntity);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(commandEntity);

            return CreatedAtRoute(nameof(GetCommandForPlatform), 
                new {platformId, commandId = commandReadDto.Id}, commandReadDto);
        }

    }
} 
