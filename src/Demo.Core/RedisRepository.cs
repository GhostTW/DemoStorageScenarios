using System.Threading.Tasks;
using Demo.Core.Lua;
using Demo.Data;
using StackExchange.Redis;

namespace Demo.Core
{
    public class RedisRepository
    {
        private const string PrefixUser = "user:";

        private const string LuaInsertUser = @"
        local key = KEYS[1]
        local id = ARGV[1]
        local code = ARGV[2]
        local password = ARGV[3]
        local isactive = ARGV[4]

        return redis.call('HSET', key, 'Id', id, 'Code', code, 'Password', password, 'IsActive', isactive)";

        private const string LuaTestConvert = @"
        redis.call('set', KEYS[1], ARGV[1])
        local val = redis.call('get', KEYS[1])
        local r = val + tonumber('100') + '10'
        return r
        ";

        private readonly LuaScriptWorker _worker;

        public RedisRepository()
        {
            _worker = new LuaScriptWorker();
            _worker.LuaScripts.Add(nameof(LuaInsertUser), LuaInsertUser);
            _worker.LuaScripts.Add(nameof(LuaTestConvert), LuaTestConvert);
        }

        public async Task<int> InsertUser(UserEntity user)
        {
            var result = await _worker.ExecuteLuaScript(
                nameof(LuaInsertUser),
                new RedisKey[] {PrefixUser + user.Id},
                new RedisValue[] {user.Id, user.Code, user.Password, user.IsActive});

            return (int) result;
        }

        public async Task<int> ConvertTest(string key, int value)
        {
            var result = await _worker.ExecuteLuaScript(
                nameof(LuaTestConvert),
                new RedisKey[] {key},
                new RedisValue[] {value});

            return (int) result;
        }
    }
}