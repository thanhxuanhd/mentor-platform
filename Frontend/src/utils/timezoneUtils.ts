import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';

// Configure dayjs plugins for timezone support
dayjs.extend(utc);
dayjs.extend(timezone);

export const convertUTCTimeSlotsToLocal = (
  utcSlots: Record<string, Array<{
    id: string;
    startTime: string;
    endTime: string;
    isAvailable: boolean;
    isBooked: boolean;
  }>>,
  userTimezone: string
): Record<string, Array<{
  id: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
  isBooked: boolean;
  originalDate: string;
  originalStartTime: string;
  originalEndTime: string;
}>> => {
  const localSlots: Record<string, Array<{
    id: string;
    startTime: string;
    endTime: string;
    isAvailable: boolean;
    isBooked: boolean;
    originalDate: string;
    originalStartTime: string;
    originalEndTime: string;
  }>> = {};

  Object.entries(utcSlots).forEach(([utcDate, slots]) => {
    slots.forEach(slot => {
      const utcStartDateTime = dayjs.utc(`${utcDate} ${slot.startTime}`);
      const utcEndDateTime = dayjs.utc(`${utcDate} ${slot.endTime}`);

      const localStartDateTime = utcStartDateTime.tz(userTimezone);
      const localEndDateTime = utcEndDateTime.tz(userTimezone);
      const localDate = localStartDateTime.format('YYYY-MM-DD');

      if (localStartDateTime.isAfter(dayjs().tz(userTimezone))) {
        if (!localSlots[localDate]) {
          localSlots[localDate] = [];
        }
        
        localSlots[localDate].push({
          id: slot.id,
          startTime: localStartDateTime.format('HH:mm'),
          endTime: localEndDateTime.format('HH:mm'),
          isAvailable: slot.isAvailable,
          isBooked: slot.isBooked,
          originalDate: utcDate,
          originalStartTime: slot.startTime,
          originalEndTime: slot.endTime
        });
      }
    });
  });

  return localSlots;
};

export const convertLocalTimeSlotsToUTC = (
  localSlots: Record<string, Array<{
    id?: string;
    startTime: string;
    endTime: string;
    isAvailable: boolean;
    isBooked: boolean;
    originalDate?: string;
    originalStartTime?: string;
    originalEndTime?: string;
  }>>,
  userTimezone: string
): Record<string, Array<{
  id?: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
  isBooked: boolean;
}>> => {
  const utcSlots: Record<string, Array<{
    id?: string;
    startTime: string;
    endTime: string;
    isAvailable: boolean;
    isBooked: boolean;
  }>> = {};

  Object.entries(localSlots).forEach(([localDate, slots]) => {
    slots.forEach(slot => {
      let utcDate: string;
      let utcStartTime: string;
      let utcEndTime: string;

      if (slot.originalDate && slot.originalStartTime && slot.originalEndTime) {
        // Use original UTC values if available
        utcDate = slot.originalDate;
        utcStartTime = slot.originalStartTime;
        utcEndTime = slot.originalEndTime;
      } else {
        // Convert local datetime to UTC
        const localStartDateTime = dayjs.tz(`${localDate} ${slot.startTime}`, userTimezone);
        const localEndDateTime = dayjs.tz(`${localDate} ${slot.endTime}`, userTimezone);
        
        const utcStartDateTime = localStartDateTime.utc();
        const utcEndDateTime = localEndDateTime.utc();
        
        utcDate = utcStartDateTime.format('YYYY-MM-DD');
        utcStartTime = utcStartDateTime.format('HH:mm');
        utcEndTime = utcEndDateTime.format('HH:mm');
      }

      if (!utcSlots[utcDate]) {
        utcSlots[utcDate] = [];
      }

      utcSlots[utcDate].push({
        id: slot.id,
        startTime: utcStartTime,
        endTime: utcEndTime,
        isAvailable: slot.isAvailable,
        isBooked: slot.isBooked
      });
    });
  });

  return utcSlots;
};

export const convertUTCTimeSlotToLocal = (
  utcDate: string,
  utcStartTime: string,
  utcEndTime: string,
  userTimezone: string
): {
  localDate: string;
  localStartTime: string;
  localEndTime: string;
} => {
  const utcStartDateTime = dayjs.utc(`${utcDate} ${utcStartTime}`);
  const utcEndDateTime = dayjs.utc(`${utcDate} ${utcEndTime}`);
  
  const localStartDateTime = utcStartDateTime.tz(userTimezone);
  const localEndDateTime = utcEndDateTime.tz(userTimezone);
  
  return {
    localDate: localStartDateTime.format('YYYY-MM-DD'),
    localStartTime: localStartDateTime.format('HH:mm'),
    localEndTime: localEndDateTime.format('HH:mm')
  };
};

export const convertLocalTimeSlotToUTC = (
  localDate: string,
  localStartTime: string,
  localEndTime: string,
  userTimezone: string
): {
  utcDate: string;
  utcStartTime: string;
  utcEndTime: string;
} => {
  const localStartDateTime = dayjs.tz(`${localDate} ${localStartTime}`, userTimezone);
  const localEndDateTime = dayjs.tz(`${localDate} ${localEndTime}`, userTimezone);
  
  const utcStartDateTime = localStartDateTime.utc();
  const utcEndDateTime = localEndDateTime.utc();
  
  return {
    utcDate: utcStartDateTime.format('YYYY-MM-DD'),
    utcStartTime: utcStartDateTime.format('HH:mm'),
    utcEndTime: utcEndDateTime.format('HH:mm')
  };
};
