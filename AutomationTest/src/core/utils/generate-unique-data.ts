export function withTimestamp(data: any) {
  const timestamp = new Date().toISOString();
  return {
    ...data,
    name: `${data.name} ${timestamp}`,
  };
}

function generateRandomString(length: number): string {
  const characters =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
  let result = "";
  const charactersLength = characters.length;

  for (let i = 0; i < length; i++) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }

  return result;
}

export function generateUniqueEmail(): string {
  const randomString = generateRandomString(5);
  return `kimcuong.${randomString}@example.com`;
}

export function generateRandomRole(): number {
  const randomNumber = Math.floor(Math.random() * 3) + 1;
  return randomNumber;
}

export function withTimestampEmail(data: any) {
  const timestamp = new Date().toISOString().replace(/[-:.TZ]/g, "");
  return {
    ...data,
    email: `${timestamp}${data.email}`,
  };
}

function formatDateToYYYYMMDD(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export function withTimestampTitleAndFutureDate(data: any, daysOffset: number = 1) {
  const timestamp = new Date().toISOString();

  let updatedDueDate = data.dueDate;
  if (data.dueDate && typeof data.dueDate === 'string') {
    const now = new Date();
    now.setDate(now.getDate() + daysOffset);
    updatedDueDate = formatDateToYYYYMMDD(now);
  }

  return {
    ...data,
    dueDate: updatedDueDate,
    title: `${data.title} ${timestamp}`,
  };
}

export function withFutureDate(data: any, daysOffset: number = 1) {
  let updatedDueDate = data.dueDate;
  if (data.dueDate && typeof data.dueDate === 'string') {
    const now = new Date();
    now.setDate(now.getDate() + daysOffset);
    updatedDueDate = formatDateToYYYYMMDD(now);
  }

  return {
    ...data,
    dueDate: updatedDueDate
  };
}


export function endWithTimestamp(original: string): string {
  return `${original}_${Date.now()}`;
}

export function titleWithTimestamp(data: any) {
  const timestamp = new Date().toISOString();
  return {
    ...data,
    title: `${data.title} ${timestamp}`,
  };
}
