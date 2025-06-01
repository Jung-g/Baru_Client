using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Baru_Client.Command
{
    internal class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<T, Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute == null || _canExecute());
        }

        public async Task ExecuteAsync(object parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                if (parameter == null && typeof(T).IsValueType)
                {
                    // 기본값 전달 (예: int -> 0)
                    await _execute(default);
                }
                else
                {
                    await _execute((T)parameter);
                }
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void Execute(object parameter)
        {
            ExecuteAsync(parameter).ConfigureAwait(false);
        }

        public event EventHandler CanExecuteChanged;

        private void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
