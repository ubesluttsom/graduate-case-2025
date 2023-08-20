import { useMsal } from "@azure/msal-react";
import { useEffect, useState } from "react";

export default function useAccessToken() {
  const { instance, accounts } = useMsal();
  const [accessToken, setAccessToken] = useState("");
  const account = accounts[0];
  
  useEffect(() => {
    if (!account) return;
    
    const request = {
      scopes: [
        import.meta.env.VITE_API_SCOPE
      ],
      account: account
    };

    instance.acquireTokenSilent(request).then((response) => {
      setAccessToken(response.accessToken);
    }).catch((e) => {
      console.warn(e);
      instance.acquireTokenRedirect(request).catch((e) => { console.warn(e); });
    }).catch((e) => {
        console.warn(e);
        setAccessToken("");
      });
    }, [account, instance ]);
  return accessToken;
}
