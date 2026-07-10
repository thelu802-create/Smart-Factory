import { useEffect, useState } from 'react';

interface ApiDataState<T> {
  data: T;
  isLoading: boolean;
  error: string | null;
  reload: () => void;
}

export function useApiData<T>(fallbackData: T, loadData: () => Promise<T>): ApiDataState<T> {
  const [data, setData] = useState(fallbackData);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [reloadIndex, setReloadIndex] = useState(0);

  useEffect(() => {
    let isMounted = true;
    setIsLoading(true);

    loadData()
      .then((nextData) => {
        if (isMounted) {
          setData(nextData);
          setError(null);
        }
      })
      .catch((loadError: unknown) => {
        if (isMounted) {
          setError(loadError instanceof Error ? loadError.message : 'Unable to load API data');
        }
      })
      .finally(() => {
        if (isMounted) {
          setIsLoading(false);
        }
      });

    return () => {
      isMounted = false;
    };
  }, [loadData, reloadIndex]);

  return { data, isLoading, error, reload: () => setReloadIndex((index) => index + 1) };
}