using Level;

namespace StateMachine.States
{
    public class LevelContext
    {
        public int LevelIndex;
        public ArenaData ArenaData;
        
        // Добавили ссылку на уникальные настройки текущего уровня
        public LevelConfigSO Config; 
    }
}