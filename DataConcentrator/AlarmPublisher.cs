using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator
{
    public class AlarmPublisher
    {
        public event Action<ActivatedAlarm> AlarmActivated;
        public event Action<ActivatedAlarm> AlarmDeactivated;

        public void AlarmsMonitoring()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(100);

                    foreach (var alarm in Context.Instance.Alarms.Local)
                    {
                        var tag = Context.Instance.Tags.FirstOrDefault(t => t.NameId == alarm.TagNameId);
                        if (tag == null || tag.OnOffScan != true)
                            continue;

                        var existing = Context.Instance.ActivatedAlarms.Local
                                            .FirstOrDefault(a => a.AlarmId == alarm.AlarmId && a.IsActive);

                        if (existing == null)
                        {
                            var activated = new ActivatedAlarm
                            {
                                AlarmId = alarm.AlarmId,
                                TagName = tag.NameId,
                                AlarmMessage = alarm.AlarmMessage
                            };

                            if (activated.AlarmActivation(tag.CurrentValue, alarm))
                            {
                                activated.IsActive = true;
                                activated.AlarmActivationTime = DateTime.Now;

                                AlarmActivated?.Invoke(activated);
                            }
                        }
                        else
                        {
                            if (existing.IsActive && existing.AlarmDeactivation(tag.CurrentValue, alarm))
                            {
                                existing.IsActive = false;
                                existing.AlarmDeactivationTime = DateTime.Now; 
                                AlarmDeactivated?.Invoke(existing);
                            }
                            else if (!existing.IsActive && existing.AlarmActivation(tag.CurrentValue, alarm))
                            {
                                existing.IsActive = true;
                                existing.AlarmActivationTime = DateTime.Now;
                                AlarmActivated?.Invoke(existing);
                            }
                        }
                    }
                }
            });
        }
    }
}

