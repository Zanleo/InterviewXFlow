using System;

namespace Cleanup
{
    internal class Program
    {
        private const double TargetChangeTime = 1;

        private double _previousTargetSetTime;
        private dynamic _lockedCandidateTarget;
        private dynamic _lockedTarget;
        private dynamic _target;
        private dynamic _previousTarget;
        private dynamic _activeTarget;
        private dynamic _targetInRangeContainer;

        private dynamic Target
        {
            set
            {
                if (_previousTarget == _target)
                {
                    return;
                }
                
                _previousTarget = _target;
                _target = value;
                if (IsValidTarget(_target))
                {
                    _previousTargetSetTime = Time.time;
                }
                TargetableEntity.Selected = _target;
            }
        }

        public void CleanupTest(Frame frame)
        {
            TryResetTarget(ref _lockedCandidateTarget, CanNotBeTarget);
            TryResetTarget(ref _lockedTarget, CanNotBeTarget);

			// Sets _activeTarget field
            // Сделал допущение!!! что тут не может быть Exception, тогда нам не нужен блок try/finally
            TrySetActiveTargetFromQuantum(frame);

            UpdateTarget();
        }

        private void UpdateTarget()
        {
            // If target exists and can be targeted, it should stay within Target Change Time since last target change
            if (CanBeTarget(_target) && IsPrevieousTargetSetTimeValid())
            {
                return;
            }
            
            // Если бы не dynamic - я бы использовал метод с такой сигнатурой , чтобы сделать и проверку и установку цель
            // private bool TrySetTarget(dynamic target, Func<dynamic, bool> checkForTarget = null)
            // Выглядело бы так
            // if (TrySetTarget(_lockedTarget, CanBeTarget)) return;
            if (CanBeTarget(_lockedTarget))
            {
                Target = _lockedTarget;
                return;
            }
            
            if (CanBeTarget(_activeTarget))
            {
                Target = _activeTarget;
                return;
            }

            // допускаю, что GetTarget() возвращает тут null если нету цели
            Target = _targetInRangeContainer.GetTarget();
        }

        // MORE CLASS CODE

        private bool IsPrevieousTargetSetTimeValid()
        {
            return Time.time - _previousTargetSetTime < TargetChangeTime;
        }

        private bool IsValidTarget(dynamic target)
        {
            return target;
        }
        
        private bool CanBeTarget(dynamic target)
        {
            return IsValidTarget(target) && target.CanBeTarget;
        }
        
        private bool CanNotBeTarget(dynamic target)
        {
            return IsValidTarget(target) && !target.CanBeTarget;
        }
        
        private bool TryResetTarget(ref dynamic lockedTarget, Func<dynamic, bool> checkForReset)
        {
            if (!checkForReset(lockedTarget))
            {
                return false;
            }
            
            lockedTarget = null;
            return true;
        }
    }
}