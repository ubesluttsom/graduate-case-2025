import {
  useAccount,
  useMsal,
} from '@azure/msal-react';
import {
  Box,
  Flex,
  Heading,
  Text,
} from '@chakra-ui/react';
import { Guest } from 'cms-types';
import { useEffect } from 'react';
import { useGet } from '../hooks/useApi';
import usePost from '../hooks/usePost';

const Home = () => {
  const { accounts } = useMsal();
  const account = useAccount(accounts[0] || {});
  const {
    data: guest,
    isLoading,
    isError,
    mutate,
  } = useGet<Guest>(`/guests/${account?.localAccountId}`);

  const post = usePost();

  useEffect(() => {
    if (isLoading) return;
    
    const checkAndCreateGuest = async () => {
      if (!isLoading && guest?.id == "" && account) {
        const newGuestData = {
          firstName: account?.name?.split(' ').slice(0, -1).join(' '),
          lastName: account?.name?.split(' ').slice(-1).join(' '),
          id: account?.localAccountId,
          email: account?.username,
        };

        const newGuest = await post('/guests', newGuestData).then((response) => { 
          if (!response.status) {
            return { id: "", firstName: "", lastName: "" };
          }
          return response.json();
        }).catch((e) => {
          console.log(e);
          return { id: "", firstName: "", lastName: "" };
        });

        mutate(newGuest, false);
      }
    };

    checkAndCreateGuest();
  }, [isLoading, guest, account, mutate, post, isError]);

  return (
    <Flex
      width="100vw"
      height="100vh"
      alignContent="center"
      justifyContent="center"
      backgroundColor="#f0f0f0"
    >
      <Box m="0 auto">
        <Heading as="h1" textAlign="center" fontSize="5xl" mt="100px">
                  Welcome, { account?.name }!
        </Heading>
        <Text fontSize="xl" textAlign="center" mt="30px">
          { guest?.id == "" && isLoading ? "Hang on, we are creating a guest account for you..." : "Your room id is " + guest?.roomId }
        </Text>
      </Box>
    </Flex>
  );
};

export default Home;
