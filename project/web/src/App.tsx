import { Box, Button, ButtonGroup, Flex, Heading, Text } from '@chakra-ui/react';
import { useState } from 'react';

function App() {
  const [count, setCount] = useState(0)

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
      </Box>
    </Flex>
  );
}

export default App
