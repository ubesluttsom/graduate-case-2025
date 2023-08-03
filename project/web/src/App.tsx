import { Box, Button, ButtonGroup, Flex, Heading, Text, Tooltip } from '@chakra-ui/react';
import { useState } from 'react';
import { useAccount, useIsAuthenticated, useMsal, useMsalAuthentication } from "@azure/msal-react";
import { InteractionType } from '@azure/msal-browser';
import useAccessToken from './auth/useAccessToken';


function App() {
  const [count, setCount] = useState(0)
  const isAuthenticated = useIsAuthenticated();
  const {error} = useMsalAuthentication(InteractionType.Redirect);
  const {accounts} = useMsal();
  const account = useAccount(accounts[0] || {});
  const accessToken = useAccessToken();

  return (
    <Flex
      width="100vw"
      height="100vh"
      alignContent="center"
      justifyContent="center"
      backgroundColor="#f0f0f0"
    >
      <Box maxW="2xl" m="0 auto">
        <Heading as="h1" textAlign="center" fontSize="5xl" mt="100px">
          Welcome!
        </Heading>
        <Text fontSize="xl" textAlign="center" mt="30px">
          Count: {count}
        </Text>
        <ButtonGroup>
          <Button
            w="fit-content"
            p="4"
            px="50px"
            colorScheme="explore-yellow"
            borderRadius="10px"
            m="0 auto"
            mt="8"
            fontWeight="bold"
            color="white"
            fontSize="l"
            onClick={() => setCount(count + 1)}
          >
            Add +1 to count
          </Button>
          <Button
            w="fit-content"
            p="4"
            px="50px"
            colorScheme="red"
            borderRadius="10px"
            m="0 auto"
            mt="8"
            fontWeight="bold"
            color="white"
            fontSize="l"
            onClick={() => setCount(count - 1)}
          >
            Remove -1 from count
          </Button>
        </ButtonGroup>
        {isAuthenticated ? InfoBlock(`Welcome ${account?.name}🎉`) : InfoBlock("You are not authenticated")}
        {error && InfoBlock(`Error: ${JSON.stringify(error)}`)}
        {accessToken && CopyToClipboardButton(accessToken, "Copy access token to clipboard")}
      </Box>
    </Flex>
  );
}

function InfoBlock(text: string) {
  return (
    <Text fontSize="xl" textAlign="center" mt="30px">
      {text}
    </Text>
  )
}

// Button to copy the access token to the clipboard
function CopyToClipboardButton(text: string, label?: string) {

  const copyToClipboard = () => {
    navigator.clipboard.writeText(text);
  }

  return (
    <Tooltip label={label ?? "Copy to clipboard"}>
      <Button
        w="fit-content"
        p="4"
        px="4px"
        colorScheme="blue"
        borderRadius="10px"
        m="0 auto"
        mt="8"
        fontWeight="bold"
        color="white"
        fontSize="l"
        onClick={copyToClipboard}
      >
        📄
      </Button>
    </Tooltip>
  )
}

export default App
