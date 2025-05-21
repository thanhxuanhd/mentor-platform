export function normalizeName(name: string): string | null {
  if (name === "") {
    return null;
  }

  const resultString = name
    .trim()
    .toLowerCase()
    .replace(/\s+/g, " ")
    .split(" ")
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
    .join(" ");

  return resultString === "" ? null : resultString;
}
