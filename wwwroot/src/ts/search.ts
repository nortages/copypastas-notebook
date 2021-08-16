export const searchParams = {};

export function search() {
  const parsedUrl = new URL(window.location.href);
  for (const [key, value] of Object.entries(searchParams)) {
    if (Array.isArray(value)) {
      parsedUrl.searchParams.delete(key);
      for (const arrVal of value) {
        parsedUrl.searchParams.append(key, arrVal.toString());
      }
    } else {
      const valueStr = value.toString();
      if (valueStr === "") {
        parsedUrl.searchParams.delete(key);
      } else {
        parsedUrl.searchParams.set(key, valueStr);

      }
    }
  }
  parsedUrl.searchParams.delete("page");
  window.location.href = parsedUrl.toString();
}