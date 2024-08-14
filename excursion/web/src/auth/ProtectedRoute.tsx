import { InteractionType } from "@azure/msal-browser";
import { useMsalAuthentication } from "@azure/msal-react";
import { Text } from '@chakra-ui/react';
import useAccessToken from "./useAccessToken";

interface ProtectedRoutesProps {
    children: React.ReactNode;
}

const ProtectedRoutes = ({ children }: ProtectedRoutesProps) => {
  const isAuthenticated = useAccessToken();
  const { error } = useMsalAuthentication(InteractionType.Redirect);

  if (!isAuthenticated) return <p>{error && InfoBlock(`Error: ${JSON.stringify(error)}`)}</p>;

  return children;
};

function InfoBlock(text: string) {
  return (
    <Text fontSize="xl" textAlign="center" mt="30px">
      {text}
    </Text>
  );
}

export default ProtectedRoutes;
