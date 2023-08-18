import useAccessToken from "../auth/useAccessToken";

const usePost = () => {
    const accessToken = useAccessToken();

    const post = (url : string, body: unknown) => {

        const response = fetch(import.meta.env.VITE_API_BASE_URL + url, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${accessToken}`
          },
          body: JSON.stringify(body)
        });

        return response;
    };

    return post;
}

export default usePost;
