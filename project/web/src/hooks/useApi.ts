import useSWR from 'swr';

const fetcher = (url: string) => fetch(url).then(res => res.json());

export function useGet<T = unknown>(path: string) {
  const { data, error, mutate, isLoading } = useSWR<T>(
    import.meta.env.VITE_API_BASE_URL + path,
    fetcher
  );

  return {
    data,
    isLoading,
    isError: error,
    mutate
  };
}
