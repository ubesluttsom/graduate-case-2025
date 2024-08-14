import useSWR from 'swr';
import useAccessToken from '../auth/useAccessToken';


export function useGet<T = unknown>(path: string, shouldFetch = true) {
  const accessToken = useAccessToken();

  
  const fetcher = (url: string) => fetch(url, {
    headers: { "Authorization": `Bearer ${accessToken}` }
  }).then(res => res.json());
  
  const { data, error, mutate, isLoading } = useSWR<T>(
    (shouldFetch && accessToken) ? import.meta.env.VITE_API_BASE_URL + path : null,
    fetcher
  );

  return {
    data,
    isLoading,
    isError: error,
    mutate
  };
}
