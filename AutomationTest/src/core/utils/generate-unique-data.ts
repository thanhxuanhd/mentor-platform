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

export function withFutureDate(data: any, daysOffset: number = 1) {
  const timestamp = new Date().toISOString();

  let updatedDueDate = data.dueDate;
  if (data.dueDate && typeof data.dueDate === 'string') {
    const now = new Date();
    now.setDate(now.getDate() + daysOffset);
    updatedDueDate = now.toISOString().split('T')[0];
  }

  return {
    ...data,
    dueDate: updatedDueDate,
    title: `${data.title} ${timestamp}`,
  };
}

